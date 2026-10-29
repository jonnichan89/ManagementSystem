
namespace ManagementSystem.DataServices.Service
{
    public class ProjectTemplateService
    {
        //SharedResources _resShared;
        //public ProjectTemplateService(SharedResources resShared) { }
        public ProjectTemplateService() { }

        private string _uploadRootFolder;
        public string UploadRootFolder { set => _uploadRootFolder = value; }
        public List<ModelValidation> CanContinueAction(ServiceAction action, ProjectTemplate mainModel)
        {
            List<ModelValidation> validations = new List<ModelValidation>();

            if (action == ServiceAction.Delete)
            {
                if (mainModel.PTId <= 0)
                    validations.Add(ModelValidation.FieldIsRequired(nameof(ProjectTemplate), nameof(mainModel.PTId), mainModel.PTId.ToString()));
            }
            else
            {
                foreach (ProjectTemplate subModel in mainModel.ProjectTemplates)
                {
                    if (string.IsNullOrWhiteSpace(subModel.PTName))
                        validations.Add(ModelValidation.FieldIsRequired(nameof(ProjectTemplate), nameof(subModel.PTName), subModel.PTName.ToString()));

                    subModel.ProjectTemplateDetails.GroupBy(d => new { d.PTId, d.ItemCode }).ToList().ForEach(g =>
                    {
                        if (g.Count() > 1)
                        {
                            validations.Add(ModelValidation.DuplicateRecord(
                                nameof(ProjectTemplateDetail),
                                nameof(ProjectTemplateDetail.ItemCode),
                                $@"{g.Key.PTId} - {g.Key.ItemCode}"));
                        }
                    });
                }
            }

            return validations;
        }

        public IEnumerable<ProjectTemplate> GetProjectTemplates()
        {
            using (var context = new ManagementSystemContext())
            {
                var items = context.
                    ProjectTemplates.
                    Include(p => p.ProjectTemplates).
                    ThenInclude(p => p.ProjectTemplateDetails).
                    Where(p => p.ParentPTId == null).
                    ToList();
                return items;
            }
        }

        public async Task<IEnumerable<ProjectTemplate>> GetAllProjectTemplatesAsync()
        {
            using (var context = new ManagementSystemContext())
            {
                return await context.ProjectTemplates.ToListAsync();
            }
        }

        public async Task<DateTime?> GetLastModifiedAsync()
        {
            using (var context = new ManagementSystemContext())
            {
                if (await context.ProjectTemplates.CountAsync() > 0)
                    return await context.ProjectTemplates.MaxAsync(p => p.ModifiedAt);
                else
                    return null;
            }
        }

        public async Task<int> CountAsync()
        {
            using (var context = new ManagementSystemContext())
            {
                return await context.ProjectTemplates.CountAsync();
            }
        }

        public bool IsExists(ServiceAction action, ProjectTemplate mainModels)
        {
            //if (action == ServiceAction.Add || action == ServiceAction.Get)
            using (var context = new ManagementSystemContext())
            {
                foreach (ProjectTemplate subModel in mainModels.ProjectTemplates)
                {
                    if (context.
                        ProjectTemplates.
                        Any(u => u.PTId != subModel.PTId && u.ParentPTId == 0 && u.PTName == subModel.PTName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsExists(ServiceAction action, ProjectTemplateDetail modelDetail)
        {
            //using (var context = new ManagementSystemContext())
            //{
            //    return context.
            //        ProjectTemplateDetails.
            //        Any(u =>
            //            u.ProjectTemplateId == subModel.ProjectTemplateId &&
            //            u.ProjectTemplateDetailId != subModel.ProjectTemplateDetailId &&
            //            u.ItemCode == subModel.ItemCode);
            //}
            return false;
        }

        public ProjectTemplate? GetProjectTemplateById(int PTId)
        {
            if (PTId == 0) throw new ArgumentNullException(nameof(PTId));

            using (var context = new ManagementSystemContext())
            {
                ProjectTemplate? item = context.
                    ProjectTemplates.
                    Include(p => p.ProjectTemplates).
                    ThenInclude(p => p.ProjectTemplateDetails).
                    SingleOrDefault(u => u.PTId == PTId);
                return item;
            }
        }

        public int CreateProjectTemplate(ProjectTemplate mainModel)
        {
            using (var context = new ManagementSystemContext())
            {
                var now = DateTime.Now;
                mainModel.CreatedAt = now;
                mainModel.ModifiedAt = now;

                foreach (var detail in mainModel.ProjectTemplateDetails)
                {
                    detail.CreatedAt = now;
                    detail.ModifiedAt = now;
                    detail.ProjectTemplate = mainModel;
                }

                foreach (var subModel in mainModel.ProjectTemplates)
                {
                    subModel.CreatedAt = now;
                    subModel.ModifiedAt = now;
                    subModel.ParentProjectTemplate = mainModel;

                    foreach (var detail in subModel.ProjectTemplateDetails)
                    {
                        detail.CreatedAt = now;
                        detail.ModifiedAt = now;
                        detail.ProjectTemplate = subModel;
                    }
                }

                // Insert
                context.ProjectTemplates.Add(mainModel);

                return context.SaveChanges();
            }
        }

        public int UpdateProjectTemplate(ProjectTemplate mainModel)
        {
                string log = "";
            try
            {
                using (var context = new ManagementSystemContext())
                {
                    var now = DateTime.Now;
                    mainModel.ModifiedAt = now;

                    log += Environment.NewLine +
                        $"Load Data PTId: {mainModel.PTId}";

                    var existing = context.
                        ProjectTemplates.
                        Include(p => p.ProjectTemplates).
                        ThenInclude(p => p.ProjectTemplateDetails).
                        SingleOrDefault(u => u.PTId == mainModel.PTId);

                    if (existing == null)
                    {
                        log += Environment.NewLine +
                            $"No Data Found.";
                        return 0;
                    }

                    // update allowed fields
                    if (!string.IsNullOrWhiteSpace(mainModel.ImagePath) ||
                        (
                            mainModel.ImageFileSize > 0 &&
                            mainModel.ImageFileSize != existing.ImageFileSize))
                    {
                        existing.ImagePath = mainModel.ImagePath;
                        existing.ImageFileSize = mainModel.ImageFileSize;
                    }

                    existing.ModifiedAt = now;
                    existing.ModifiedBy = mainModel.ModifiedBy;

                    //List<int?[]> PTToDelete = new List<int?[]>();
                    List<ProjectTemplateDetail> PTDToDelete = new List<ProjectTemplateDetail>();

                    // ----------------------------------------------------------
                    log += Environment.NewLine +
                        $"Compare Existing.PTD with subModelPT.PTD to Delete.";

                    foreach (var subModelPT in mainModel.ProjectTemplates)
                    {
                        PTDToDelete.Clear();

                        var existingPT = existing.ProjectTemplates.FirstOrDefault(p => p.PTId == subModelPT.PTId);
                        if (existingPT != null)
                        {
                            existingPT.
                                ProjectTemplateDetails.
                                Where(d => !subModelPT.ProjectTemplateDetails.Any(nd => nd.PTId == d.PTId && nd.PTDId == d.PTDId)).
                                ToList().
                                ForEach(d =>
                                {
                                    log += Environment.NewLine +
                                        $"Existing.PTD to delete: PTId = {d.PTId} AND PTDId = {d.PTDId}.";

                                    PTDToDelete.Add(d);

                                    // remove details that are not in the updated item
                                    log += Environment.NewLine +
                                        $"Removing PTD from Context: PTId = {d.PTId} AND PTDId = {d.PTDId}.";
                                    context.ProjectTemplateDetails.Remove(d);
                                    context.SaveChanges();
                                });
                        }

                        log += Environment.NewLine +
                            $"Removing PTD from existing: Count = {PTDToDelete.Count}.";

                        PTDToDelete.ForEach(d => existingPT.ProjectTemplateDetails.Remove(d));
                    }

                    // ----------------------------------------------------------
                    log += Environment.NewLine +
                        $"Compare Existing.PT with mainModel.PT to Delete.";

                    List<ProjectTemplate> PTToDelete = new List<ProjectTemplate>();
                    existing.
                        ProjectTemplates.
                        Where(d => !mainModel.ProjectTemplates.Any(nd => nd.PTId == d.PTId)).
                        ToList().
                        ForEach(d =>
                        {
                            log += Environment.NewLine +
                                $"Existing.PT to delete: PTId = {d.PTId}.";

                            if (d.ImagePath != "")
                            {
                                if (System.IO.File.Exists(System.IO.Path.Combine(_uploadRootFolder, d.ImagePath)))
                                {
                                    System.IO.File.Delete(System.IO.Path.Combine(_uploadRootFolder, d.ImagePath));
                                }
                            }

                            //PTToDelete.Add(new int?[] { d.PTId, d.ParentPTId });
                            PTToDelete.Add(d);

                            // remove details that are not in the updated item
                            log += Environment.NewLine +
                                $"Removing PT from Context: PTId = {d.PTId}.";
                            context.ProjectTemplates.Remove(d);
                            context.SaveChanges();
                        });

                    log += Environment.NewLine +
                        $"Removing PT from existing: Count = {PTToDelete.Count}.";

                    PTToDelete.ForEach(d => existing.ProjectTemplates.Remove(d));

                    // ----------------------------------------------------------
                    log += Environment.NewLine +
                        $"Loop mainModel.PT to Insert / Update.";
                    bool newSubPT = false;
                    byte PTNo = 0;
                    int PTDId = 0;
                    foreach (var subModel in mainModel.ProjectTemplates.OrderBy(p => p.PTNo))
                    {
                        PTNo++;
                        PTDId = 0;

                        subModel.PTNo = PTNo;

                        log += Environment.NewLine +
                            $"  subModel PTId = {subModel.PTId} AND PTNo = {subModel.PTNo}.";

                        log += Environment.NewLine +
                            $"  Find existingSubModel from subModel PTId = {subModel.PTId}.";
                        var existingModel =
                            existing.
                            ProjectTemplates.
                            SingleOrDefault(d => d.PTId == subModel.PTId);

                        if (existingModel != null)
                        {
                            log += Environment.NewLine +
                            $"      Find existingSubModel found.";

                            newSubPT = false;

                            // update allowed fields
                            if (string.Concat(existingModel.PTNo, existingModel.PTName, existingModel.ImagePath, existingModel.ImageFileSize, existingModel.Note) !=
                                string.Concat(subModel.PTNo, subModel.PTName, subModel.ImagePath, subModel.ImageFileSize, subModel.Note))
                            {
                                log += Environment.NewLine +
                                $"      Update existingSubModel properties.";

                                existingModel.PTNo = subModel.PTNo;
                                existingModel.PTName = subModel.PTName;
                                existingModel.ImagePath = subModel.ImagePath;
                                existingModel.ImageFileSize = subModel.ImageFileSize;
                                existingModel.Note = subModel.Note;
                                existingModel.ModifiedAt = DateTime.Now;
                                existingModel.ModifiedBy = subModel.ModifiedBy;
                            }
                        }
                        else
                        {
                            log += Environment.NewLine +
                                $"      New existingSubModel PTNo = {PTNo}.";

                            // new detail
                            newSubPT = true;

                            subModel.PTNo = PTNo;

                            subModel.ParentPTId = existing.PTId;
                            subModel.ParentProjectTemplate = existing;

                            subModel.CreatedAt = DateTime.Now;
                            subModel.ModifiedAt = DateTime.Now;
                        }

                        PTDId = 0;
                        foreach (var subModelPTD in subModel.ProjectTemplateDetails)
                        {
                            PTDId++;
                            subModelPTD.PTId = subModel.PTId;
                            subModelPTD.PTDId = PTDId;

                            log += Environment.NewLine +
                                $"  subModelPTD PTId = {subModelPTD.PTId} AND PTDId = {subModelPTD.PTDId}.";

                            if (newSubPT)
                            {
                                log += Environment.NewLine +
                                    $"      New subModelPTD PTId = {subModelPTD.PTId} AND PTDId = {PTDId}.";

                                // new detail
                                subModelPTD.CreatedAt = DateTime.Now;
                                subModelPTD.ModifiedAt = DateTime.Now;
                            }
                            else
                            {
                                var existingPTD =
                                    existingModel.
                                    ProjectTemplateDetails.
                                    SingleOrDefault(d => d.PTDId == subModelPTD.PTDId);

                                if (existingPTD != null)
                                {
                                    log += Environment.NewLine +
                                        $"      existingPTD Found PTDId = {PTDId}.";

                                    // update allowed fields
                                    if (string.Concat(existingPTD.ItemCode, existingPTD.Qty, existingPTD.Note) !=
                                        string.Concat(subModelPTD.ItemCode, subModelPTD.Qty, subModelPTD.Note))
                                    {
                                        existingPTD.ItemCode = subModelPTD.ItemCode;
                                        existingPTD.ItemName = subModelPTD.ItemName;
                                        existingPTD.Qty = subModelPTD.Qty;
                                        existingPTD.Note = subModelPTD.Note;
                                        existingPTD.ModifiedAt = DateTime.Now;
                                        existingPTD.ModifiedBy = subModelPTD.ModifiedBy;
                                    }
                                }
                                else
                                {
                                    log += Environment.NewLine +
                                        $"      new subModelPTD, PTDId = {PTDId}.";

                                    // new detail
                                    subModelPTD.ProjectTemplate = existingModel;

                                    subModelPTD.PTId = 0;

                                    subModelPTD.CreatedAt = DateTime.Now;
                                    subModelPTD.ModifiedAt = DateTime.Now;

                                    existingModel.ProjectTemplateDetails.Add(subModelPTD);
                                }
                            }
                        }

                        if (newSubPT)
                            existing.ProjectTemplates.Add(subModel);
                    }

                    log += Environment.NewLine +
                        $"      update existing into DB.";
                    context.ProjectTemplates.Update(existing);

                    log += Environment.NewLine +
                        $"      context Save Changes.";
                    return context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log += Environment.NewLine +
                    $"      {ex.Message}.";

                int exCount = 0;
                Exception ex1;
                while (ex.InnerException != null)
                {
                    exCount++;
                    ex1 = ex.InnerException;

                    log += Environment.NewLine +
                        $"      ex {exCount}: {ex1.Message}.";
                }

                System.IO.File.WriteAllText("C:\\Temp\\MS-Error.txt", log);
                throw;
            }
        }

        public ProjectTemplate? DeleteProjectTemplate(int PTId)
        {
            if (PTId == 0) throw new ArgumentNullException(nameof(PTId));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.
                    ProjectTemplates.
                    Include(p => p.ProjectTemplates).
                    SingleOrDefault(u => u.PTId == PTId);

                if (existing == null) goto ReturnResult;

                foreach (var existingPT in existing.ProjectTemplates)
                {
                    context.ProjectTemplates.Remove(existingPT);
                    context.SaveChanges();

                    if (existingPT.ImagePath != "")
                    {
                        if (System.IO.File.Exists(System.IO.Path.Combine(_uploadRootFolder, existingPT.ImagePath)))
                        {
                            System.IO.File.Delete(System.IO.Path.Combine(_uploadRootFolder, existingPT.ImagePath));
                        }
                    }
                }

                context.ProjectTemplates.Remove(existing);
                if (context.SaveChanges() > 0)
                {
                    return existing;
                }
            }

        ReturnResult:
            return null;
        }
    }
}
