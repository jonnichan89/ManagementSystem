
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

            if (string.IsNullOrWhiteSpace(model.SupplierCode))
                validations.Add(ModelValidation.FieldIsRequired(nameof(MstSupplier), nameof(model.SupplierCode), model.SupplierCode));

            // Validasi taro dibawah karena Delete cuma terisi PK nya saja
            if (action != ServiceAction.Delete)
            {
                if (string.IsNullOrWhiteSpace(model.SupplierName))
                    validations.Add(ModelValidation.FieldIsRequired(nameof(MstSupplier), nameof(model.SupplierName), model.SupplierName));

                if (IsExists(action, model))
                    validations.Add(ModelValidation.PKAlreadyExists(nameof(MstSupplier), nameof(model.SupplierCode), model.SupplierCode));

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

        public bool IsExists(ServiceAction action, MstSupplier model)
        {
            if (action == ServiceAction.Add || action == ServiceAction.Get)
                using (var context = new ManagementSystemContext())
                {
                    return context.MstSuppliers.Any(u => u.SupplierId != model.SupplierId && u.SupplierCode == model.SupplierCode);
                }

            return false;
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
