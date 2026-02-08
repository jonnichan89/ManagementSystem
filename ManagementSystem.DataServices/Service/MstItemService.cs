
namespace ManagementSystem.DataServices.Service
{
    public class MstItemService
    {
        //SharedResources _resShared;
        //public MstItemService(SharedResources resShared) { }
        public MstItemService() { }

        public List<ModelValidation> CanContinueAction(ServiceAction action, MstItem model)
        {
            List<ModelValidation> validations = new List<ModelValidation>();

            if (string.IsNullOrWhiteSpace(model.ItemCode))
                validations.Add(ModelValidation.FieldIsRequired(nameof(MstItem), nameof(model.ItemCode), model.ItemCode));

            // Validasi taro dibawah karena Delete cuma terisi PK nya saja
            if (action != ServiceAction.Delete)
            {
                if (string.IsNullOrWhiteSpace(model.ItemName))
                    validations.Add(ModelValidation.FieldIsRequired(nameof(MstItem), nameof(model.ItemName), model.ItemName));

                if (IsExists(action, model))
                    validations.Add(ModelValidation.PKAlreadyExists(nameof(MstItem), nameof(model.ItemCode), model.ItemCode));

            }

            return validations;
        }

        public IEnumerable<MstItem> GetMstItems()
        {
            using (var context = new ManagementSystemContext())
            {
                var items = context.MstItems.ToList();
                return items;
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
