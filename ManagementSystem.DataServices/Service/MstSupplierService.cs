
namespace ManagementSystem.DataServices.Service
{
    public class MstSupplierService
    {
        //SharedResources _resShared;
        //public MstSupplierService(SharedResources resShared) { }
        public MstSupplierService() { }

        public List<ModelValidation> CanContinueAction(ServiceAction action, MstSupplier model)
        {
            List<ModelValidation> validations = new List<ModelValidation>();

            if (action == ServiceAction.Delete)
            {
                if (model.SupplierId <= 0)
                    validations.Add(ModelValidation.FieldIsRequired(nameof(MstSupplier), nameof(model.SupplierId), model.SupplierId.ToString()));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.SupplierCode))
                    validations.Add(ModelValidation.FieldIsRequired(nameof(MstSupplier), nameof(model.SupplierCode), model.SupplierCode));

                if (string.IsNullOrWhiteSpace(model.SupplierName))
                    validations.Add(ModelValidation.FieldIsRequired(nameof(MstSupplier), nameof(model.SupplierName), model.SupplierName));

                List<ModelValidation> validations1 = IsExists(action, model);
                if (validations1.Count > 0)
                    validations.AddRange(validations1);
            }

            return validations;
        }

        public IEnumerable<MstSupplier> GetMstSuppliers()
        {
            using (var context = new ManagementSystemContext())
            {
                var suppliers = context.MstSuppliers.ToList();
                return suppliers;
            }
        }

        public async Task<IEnumerable<MstSupplier>> GetAllMstSuppliers()
        {
            using (var context = new ManagementSystemContext())
            {
                return await context.MstSuppliers.ToListAsync();
            }
        }

        public async Task<DateTime?> GetLastModifiedAsync()
        {
            using (var context = new ManagementSystemContext())
            {
                if (await context.MstSuppliers.CountAsync() > 0)
                    return await context.MstSuppliers.MaxAsync(p => p.ModifiedAt);
                else
                    return null;
            }
        }

        public List<ModelValidation> IsExists(ServiceAction action, MstSupplier model)
        {
            List<ModelValidation> validations = new List<ModelValidation>();
            if (action == ServiceAction.Add || action == ServiceAction.Update)
                using (var context = new ManagementSystemContext())
                {
                    if (context.MstSuppliers.Any(u => u.SupplierId != model.SupplierId && u.SupplierCode == model.SupplierCode))
                        validations.Add(ModelValidation.PKAlreadyExists(nameof(MstSupplier), nameof(model.SupplierCode), model.SupplierCode));
                }

            return validations;
        }

        public MstSupplier? GetMstSupplierById(int supplierId)
        {
            if (supplierId == 0) throw new ArgumentNullException(nameof(supplierId));

            using (var context = new ManagementSystemContext())
            {
                MstSupplier? supplier = context.MstSuppliers.SingleOrDefault(u => u.SupplierId == supplierId);
                return supplier;
            }
        }

        public int CreateMstSupplier(MstSupplier supplier)
        {
            using (var context = new ManagementSystemContext())
            {
                var now = DateTime.Now;
                supplier.CreatedAt = now;
                supplier.ModifiedAt = now;

                context.MstSuppliers.Add(supplier);
                return context.SaveChanges();
            }
        }

        public int UpdateMstSupplier(MstSupplier supplier)
        {
            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstSuppliers.SingleOrDefault(u => u.SupplierId == supplier.SupplierId);
                if (existing == null) return 0;

                // update allowed fields
                existing.SupplierCode = supplier.SupplierCode;
                existing.SupplierName = supplier.SupplierName;
                existing.UsedInPO = supplier.UsedInPO;
                existing.UsedInSO = supplier.UsedInSO;
                existing.UsedInPPO = supplier.UsedInPPO;
                existing.UsedInPSO = supplier.UsedInPSO;
                existing.IsBlacklist = supplier.IsBlacklist;
                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = supplier.ModifiedBy;

                context.MstSuppliers.Update(existing);

                return context.SaveChanges();
            }
        }

        public bool DeleteMstSupplier(int supplierId)
        {
            if (supplierId == 0) throw new ArgumentNullException(nameof(supplierId));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstSuppliers.SingleOrDefault(u => u.SupplierId == supplierId);
                if (existing == null) return false;

                context.MstSuppliers.Remove(existing);
                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Soft-delete by marking IsBlacklist = true and updating ModifiedAt/ModifiedBy.
        /// Returns true when save succeeded and supplier existed.
        /// </summary>
        public bool BlacklistSupplier(bool value, int supplierId, string modifiedBy)
        {
            if (supplierId == 0) throw new ArgumentNullException(nameof(supplierId));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstSuppliers.SingleOrDefault(u => u.SupplierId == supplierId);
                if (existing == null) return false;

                existing.IsBlacklist = value;
                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = modifiedBy;

                context.MstSuppliers.Update(existing);
                return context.SaveChanges() > 0;
            }
        }
    }
}
