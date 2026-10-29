
namespace ManagementSystem.DataServices.Service
{
    public class ItemAdjustmentService : BaseService
    {
        //SharedResources _resShared;
        //public ItemAdjustmentService(SharedResources resShared) { }
        public ItemAdjustmentService() { }

        public List<ModelValidation> CanContinueAction(ServiceAction action, ItemAdjustment model)
        {
            List<ModelValidation> validations = new List<ModelValidation>();

            if (action == ServiceAction.Delete)
            {
                if (model.IAId <= 0)
                    validations.Add(ModelValidation.FieldIsRequired(nameof(ItemAdjustment), nameof(model.IAId), model.IAId.ToString()));
            }
            else
            {
                model.ItemAdjustmentDetails.GroupBy(d => d.ItemCode).ToList().ForEach(g =>
                {
                    if (g.Count() > 1)
                    {
                        validations.Add(ModelValidation.DuplicateRecord(
                            nameof(ItemAdjustmentDetail),
                            nameof(ItemAdjustmentDetail.ItemCode),
                            g.Key));
                    }
                });

                if (IsExists(action, model))
                    validations.Add(ModelValidation.PKAlreadyExists(nameof(ItemAdjustment), nameof(model.IADate), model.IADate.ToString("dd MMM yyyy")));
            }

            return validations;
        }

        public IEnumerable<ItemAdjustment> GetItemAdjustments()
        {
            using (var context = new ManagementSystemContext())
            {
                var items = context.
                    ItemAdjustments.
                    Include(p => p.ItemAdjustmentDetails).
                    ToList();
                return items;
            }
        }

        public bool IsExists(ServiceAction action, ItemAdjustment model)
        {
            if (action == ServiceAction.Add || action == ServiceAction.Get)
                using (var context = new ManagementSystemContext())
                {
                    return context.
                        ItemAdjustments.
                        Include(p => p.ItemAdjustmentDetails).
                        Any(u => u.IAId != model.IAId && u.IADate == model.IADate);
                }

            return false;
        }

        public bool IsExists(ServiceAction action, ItemAdjustmentDetail model)
        {
            using (var context = new ManagementSystemContext())
            {
                return context.
                    ItemAdjustmentDetails.
                    Any(u =>
                        u.IAId == model.IAId &&
                        u.IADId != model.IADId &&
                        u.ItemCode == model.ItemCode);
            }
        }

        public ItemAdjustment? GetItemAdjustmentById(int IAId)
        {
            if (IAId == 0) throw new ArgumentNullException(nameof(IAId));

            using (var context = new ManagementSystemContext())
            {
                ItemAdjustment? item = context.
                    ItemAdjustments.
                    Include(p => p.ItemAdjustmentDetails).
                    SingleOrDefault(u => u.IAId == IAId);
                return item;
            }
        }

        public int CreateItemAdjustment(ItemAdjustment item)
        {
            using (var context = new ManagementSystemContext())
            {
                var now = DateTime.Now;
                item.CreatedAt = now;
                item.ModifiedAt = now;

                foreach (var detail in item.ItemAdjustmentDetails)
                {
                    detail.ItemAdjustment = item;
                }

                context.ItemAdjustments.Add(item);

                int rowsAffected = context.SaveChanges();
                if (rowsAffected > 0)
                {
                    ExecuteRecalItemCalc(context, item.IADate);
                }

                return rowsAffected;

            }
        }

        public int UpdateItemAdjustment(ItemAdjustment item)
        {
            using (var context = new ManagementSystemContext())
            {
                var existing = context.
                    ItemAdjustments.
                    Include(p => p.ItemAdjustmentDetails).
                    SingleOrDefault(u => u.IAId == item.IAId);

                if (existing == null) return 0;

                // update allowed fields
                existing.IADate = item.IADate;
                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = item.ModifiedBy;

                existing.ItemAdjustmentDetails.Where(d => !item.ItemAdjustmentDetails.Any(nd => nd.IADId == d.IADId)).ToList().ForEach(d =>
                {
                    // remove details that are not in the updated item
                    context.ItemAdjustmentDetails.Remove(d);
                });

                foreach (var detail in item.ItemAdjustmentDetails)
                {
                    var existingDetail =
                        existing.
                        ItemAdjustmentDetails.
                        SingleOrDefault(d => d.IADId == detail.IADId);

                    if (existingDetail != null)
                    {
                        // update allowed fields
                        existingDetail.ItemCode = detail.ItemCode;
                        existingDetail.Qty = detail.Qty;
                        existingDetail.ModifiedAt = DateTime.Now;
                        existingDetail.ModifiedBy = detail.ModifiedBy;
                    }
                    else
                    {
                        // new detail
                        detail.CreatedAt = DateTime.Now;
                        detail.ModifiedAt = DateTime.Now;
                        detail.ItemAdjustment = existing;
                        existing.ItemAdjustmentDetails.Add(detail);
                    }
                }

                context.ItemAdjustments.Update(existing);

                int rowsAffected = context.SaveChanges();
                if (rowsAffected > 0)
                {
                    ExecuteRecalItemCalc(context, existing.IADate);
                }

                return rowsAffected;

            }
        }

        public ItemAdjustment? DeleteItemAdjustment(int IAId)
        {
            if (IAId == 0) throw new ArgumentNullException(nameof(IAId));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.ItemAdjustments.SingleOrDefault(u => u.IAId == IAId);
                if (existing == null) goto ReturnResult;

                context.ItemAdjustments.Remove(existing);

                int rowsAffected = context.SaveChanges();
                if (rowsAffected > 0)
                {
                    ExecuteRecalItemCalc(context, existing.IADate);
                }

                return existing;

            }

        ReturnResult:
            return null;
        }
    }
}
