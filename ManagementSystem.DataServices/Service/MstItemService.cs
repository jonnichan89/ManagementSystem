using Dapper;
using ManagementSystem.Common.OtherModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Data;

namespace ManagementSystem.DataServices.Service
{
    public class MstItemService
    {
        //SharedResources _resShared;
        //public MstItemService(SharedResources resShared) { }
        public MstItemService() { }

        private string _uploadRootFolder;
        public string UploadRootFolder { set => _uploadRootFolder = value; }

        public List<ModelValidation> CanContinueAction(ServiceAction action, MstItem model)
        {
            List<ModelValidation> validations = new List<ModelValidation>();

            if (action == ServiceAction.Delete)
            {
                if (model.ItemId <= 0)
                    validations.Add(ModelValidation.FieldIsRequired(nameof(MstItem), nameof(model.ItemId), model.ItemId.ToString()));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.ItemCode))
                    validations.Add(ModelValidation.FieldIsRequired(nameof(MstItem), nameof(model.ItemCode), model.ItemCode));

                if (string.IsNullOrWhiteSpace(model.ItemName))
                    validations.Add(ModelValidation.FieldIsRequired(nameof(MstItem), nameof(model.ItemName), model.ItemName));

                if (IsExists(action, model))
                    validations.Add(ModelValidation.PKAlreadyExists(nameof(MstItem), nameof(model.ItemCode), model.ItemCode));
            }

            return validations;
        }

        public async Task<IEnumerable<MstItem>> GetMstItems(int pageIndex, PageFilterMstItem? pageFilter)
        {
            List<MstItem> items = null;

            using (var context = new ManagementSystemContext())
            {
                if (pageFilter == null)
                {
                    pageFilter = new PageFilterMstItem();
                }

                pageFilter.PageIndex = pageIndex;

                string query = @"
/*
DECLARE @ItemCode VARCHAR(50) = ''
DECLARE @ItemName VARCHAR(50) = ''
DECLARE @ItemDescription VARCHAR(50) = ''
DECLARE @PageIndex INT = 1
DECLARE @RecordPerPage INT = 20
*/

DECLARE @OffsetRows INT = (@PageIndex - 1) * @RecordPerPage;

IF @ItemCode <> ''
	SET @ItemCode = '%' + @ItemCode + '%';
IF @ItemName <> ''
	SET @ItemName = '%' + @ItemName + '%';
IF @ItemDescription <> ''
	SET @ItemDescription = '%' + @ItemDescription + '%';

SELECT I.*
FROM MstItem I
WHERE (@ItemCode = '' OR (@ItemCode <> '' AND I.ItemCode LIKE @ItemCode))
AND (@ItemName = '' OR (@ItemName <> '' AND I.ItemName LIKE @ItemName))
AND (@ItemDescription = '' OR (@ItemDescription <> '' AND I.ItemDescription LIKE @ItemDescription))
ORDER BY I.CreatedAt DESC
OFFSET @OffsetRows ROWS
FETCH NEXT @RecordPerPage ROWS ONLY
";

                using (SqlConnection sqlConnection = context.Database.GetDbConnection() as SqlConnection)
                {
                    if (sqlConnection.State != ConnectionState.Open)
                    {
                        sqlConnection.Open();
                    }

                    var queryResult = await sqlConnection.QueryAsync<MstItem>(query, pageFilter);
                    items = queryResult.ToList();

                    sqlConnection.Close();
                }
            }
            return items;
        }

        public async Task<IEnumerable<MstItem>> GetAllMstItems()
        {
            using (var context = new ManagementSystemContext())
            {
                return await context.MstItems.ToListAsync();
            }
        }

        public async Task<DateTime?> GetLastModifiedAsync()
        {
            using (var context = new ManagementSystemContext())
            {
                if (await context.MstItems.CountAsync() > 0)
                    return await context.MstItems.MaxAsync(p => p.ModifiedAt);
                else
                    return null;
            }
        }

        public async Task<int> CountAsync()
        {
            using (var context = new ManagementSystemContext())
            {
                return await context.MstItems.CountAsync();
            }
        }

        public bool IsExists(ServiceAction action, MstItem model)
        {
            if (action == ServiceAction.Add || action == ServiceAction.Get)
                using (var context = new ManagementSystemContext())
                {
                    return context.MstItems.Any(u => u.ItemId != model.ItemId && u.ItemCode == model.ItemCode);
                }

            return false;
        }

        public MstItem? GetMstItemById(int itemId)
        {
            if (itemId == 0) throw new ArgumentNullException(nameof(itemId));

            using (var context = new ManagementSystemContext())
            {
                MstItem? item = context.MstItems.SingleOrDefault(u => u.ItemId == itemId);
                return item;
            }
        }

        public int CreateMstItem(MstItem item)
        {
            using (var context = new ManagementSystemContext())
            {
                var now = DateTime.Now;
                item.CreatedAt = now;
                item.ModifiedAt = now;

                context.MstItems.Add(item);
                return context.SaveChanges();
            }
        }

        public int UpdateMstItem(MstItem item)
        {
            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstItems.SingleOrDefault(u => u.ItemId == item.ItemId);
                if (existing == null) return 0;

                // update allowed fields
                existing.ItemCode = item.ItemCode;
                existing.ItemName = item.ItemName;
                existing.ItemDescription = item.ItemDescription;
                existing.ItemUnit = item.ItemUnit;
                existing.Colour = item.Colour;
                if (item.HasImageUpload)
                {
                    existing.ImagePath = item.ImagePath;
                    existing.ImageFileSize = item.ImageFileSize;
                }
                existing.SupplierCode = item.SupplierCode;
                existing.IsStockControl = item.IsStockControl;
                existing.IsBlacklist = item.IsBlacklist;
                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = item.ModifiedBy;

                context.MstItems.Update(existing);

                return context.SaveChanges();
            }
        }

        public MstItem? DeleteMstItem(int itemId)
        {
            if (itemId == 0) throw new ArgumentNullException(nameof(itemId));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstItems.SingleOrDefault(u => u.ItemId == itemId);
                if (existing == null) goto ReturnResult;

                context.MstItems.Remove(existing);
                if (context.SaveChanges() > 0)
                {
                    if (existing.ImagePath != "")
                    {
                        if (System.IO.File.Exists(System.IO.Path.Combine(_uploadRootFolder, existing.ImagePath)))
                        {
                            System.IO.File.Delete(System.IO.Path.Combine(_uploadRootFolder, existing.ImagePath));
                        }
                    }

                    return existing;
                }
            }

        ReturnResult:
            return null;
        }

        /// <summary>
        /// Soft-delete by marking IsBlacklist = true and updating ModifiedAt/ModifiedBy.
        /// Returns true when save succeeded and item existed.
        /// </summary>
        public bool BlacklistItem(bool value, int itemId, string modifiedBy)
        {
            if (itemId == 0) throw new ArgumentNullException(nameof(itemId));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstItems.SingleOrDefault(u => u.ItemId == itemId);
                if (existing == null) return false;

                existing.IsBlacklist = value;
                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = modifiedBy;

                context.MstItems.Update(existing);
                return context.SaveChanges() > 0;
            }
        }
    }
}
