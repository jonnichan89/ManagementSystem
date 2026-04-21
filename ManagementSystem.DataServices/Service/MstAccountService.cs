
namespace ManagementSystem.DataServices.Service
{
    public class MstAccountService
    {
        //SharedResources _resShared;
        //public MstAccountService(SharedResources resShared) { }
        public MstAccountService() { }

        public List<ModelValidation> CanContinueAction(ServiceAction action, MstAccount model)
        {
            List<ModelValidation> validations = new List<ModelValidation>();

            if (action == ServiceAction.Delete)
            {
                if (model.AccountId <= 0)
                    validations.Add(ModelValidation.FieldIsRequired(nameof(MstAccount), nameof(model.AccountId), model.AccountId.ToString()));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.AccountCode))
                    validations.Add(ModelValidation.FieldIsRequired(nameof(MstAccount), nameof(model.AccountCode), model.AccountCode));

                if (string.IsNullOrWhiteSpace(model.AccountName))
                    validations.Add(ModelValidation.FieldIsRequired(nameof(MstAccount), nameof(model.AccountName), model.AccountName));

                if (IsExists(action, model))
                    validations.Add(ModelValidation.PKAlreadyExists(nameof(MstAccount), nameof(model.AccountCode), model.AccountCode));
            }

            return validations;
        }

        public IEnumerable<MstAccount> GetMstAccounts()
        {
            using (var context = new ManagementSystemContext())
            {
                var suppliers = context.MstAccounts.ToList();
                return suppliers;
            }
        }

        public bool IsExists(ServiceAction action, MstAccount model)
        {
            if (action == ServiceAction.Add || action == ServiceAction.Get)
                using (var context = new ManagementSystemContext())
                {
                    return context.MstAccounts.Any(u => u.AccountId != model.AccountId && u.AccountCode == model.AccountCode);
                }

            return false;
        }

        public MstAccount? GetMstAccountById(int supplierId)
        {
            if (supplierId == 0) throw new ArgumentNullException(nameof(supplierId));

            using (var context = new ManagementSystemContext())
            {
                MstAccount? supplier = context.MstAccounts.SingleOrDefault(u => u.AccountId == supplierId);
                return supplier;
            }
        }

        public int CreateMstAccount(MstAccount supplier)
        {
            using (var context = new ManagementSystemContext())
            {
                var now = DateTime.Now;
                supplier.CreatedAt = now;
                supplier.ModifiedAt = now;

                context.MstAccounts.Add(supplier);
                return context.SaveChanges();
            }
        }

        public int UpdateMstAccount(MstAccount supplier)
        {
            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstAccounts.SingleOrDefault(u => u.AccountId == supplier.AccountId);
                if (existing == null) return 0;

                // update allowed fields
                existing.AccountCode = supplier.AccountCode;
                existing.AccountName = supplier.AccountName;
                existing.ModuleType = supplier.ModuleType;
                existing.DebitNameFormat = supplier.DebitNameFormat;
                existing.CreditNameFormat = supplier.CreditNameFormat;
                existing.IsBlacklist = supplier.IsBlacklist;
                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = supplier.ModifiedBy;

                context.MstAccounts.Update(existing);

                return context.SaveChanges();
            }
        }

        public bool DeleteMstAccount(int supplierId)
        {
            if (supplierId == 0) throw new ArgumentNullException(nameof(supplierId));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstAccounts.SingleOrDefault(u => u.AccountId == supplierId);
                if (existing == null) return false;

                context.MstAccounts.Remove(existing);
                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Soft-delete by marking IsBlacklist = true and updating ModifiedAt/ModifiedBy.
        /// Returns true when save succeeded and supplier existed.
        /// </summary>
        public bool BlacklistAccount(bool value, int supplierId, string modifiedBy)
        {
            if (supplierId == 0) throw new ArgumentNullException(nameof(supplierId));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.MstAccounts.SingleOrDefault(u => u.AccountId == supplierId);
                if (existing == null) return false;

                existing.IsBlacklist = value;
                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = modifiedBy;

                context.MstAccounts.Update(existing);
                return context.SaveChanges() > 0;
            }
        }
    }
}
