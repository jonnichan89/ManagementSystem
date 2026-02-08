
namespace ManagementSystem.DataServices.Service
{
    public class ItemUsageService
    {
        //SharedResources _resShared;
        //public ItemUsageService(SharedResources resShared) { }
        public ItemUsageService() { }

        public List<ModelValidation> CanContinueAction(ServiceAction action, ItemUsage model)
        {
            List<ModelValidation> validations = new List<ModelValidation>();

            model.ItemUsageDetails.GroupBy(d => d.ItemCode).ToList().ForEach(g =>
            {
                if (g.Count() > 1)
                {
                    validations.Add(ModelValidation.DuplicateRecord(
                        nameof(ItemUsageDetail),
                        nameof(ItemUsageDetail.ItemCode),
                        g.Key));
                }
            });

            // Validasi taro dibawah karena Delete cuma terisi PK nya saja
            if (action != ServiceAction.Delete)
            {
                if (IsExists(action, model))
                    validations.Add(ModelValidation.PKAlreadyExists(nameof(ItemUsage), nameof(model.ItemUsageDate), model.ItemUsageDate.ToString("dd MMM yyyy")));
            }

            return validations;
        }

        public IEnumerable<ItemUsage> GetItemUsages()
        {
            using (var context = new ManagementSystemContext())
            {
                var items = context.
                    ItemUsages.
                    Include(p => p.ItemUsageDetails).
                    ToList();
                return items;
            }
        }

        public bool IsExists(ServiceAction action, ItemUsage model)
        {
            if (action == ServiceAction.Add || action == ServiceAction.Get)
                using (var context = new ManagementSystemContext())
                {
                    return context.
                        ItemUsages.
                        Include(p => p.ItemUsageDetails).
                        Any(u => u.ItemUsageId != model.ItemUsageId && u.ItemUsageDate == model.ItemUsageDate);
                }

            return false;
        }

        public bool IsExists(ServiceAction action, ItemUsageDetail model)
        {
            using (var context = new ManagementSystemContext())
            {
                return context.
                    ItemUsageDetails.
                    Any(u =>
                        u.ItemUsageId == model.ItemUsageId &&
                        u.ItemUsageDetailId != model.ItemUsageDetailId &&
                        u.ItemCode == model.ItemCode);
            }
        }

        public ItemUsage? GetItemUsageById(int itemUsageId)
        {
            if (itemUsageId == 0) throw new ArgumentNullException(nameof(itemUsageId));

            using (var context = new ManagementSystemContext())
            {
                ItemUsage? item = context.
                    ItemUsages.
                    Include(p => p.ItemUsageDetails).
                    SingleOrDefault(u => u.ItemUsageId == itemUsageId);
                return item;
            }
        }

        public int CreateItemUsage(ItemUsage item)
        {
            using (var context = new ManagementSystemContext())
            {
                var now = DateTime.Now;
                item.CreatedAt = now;
                item.ModifiedAt = now;

                foreach (var detail in item.ItemUsageDetails)
                {
                    detail.ItemUsage = item;
                }

                context.ItemUsages.Add(item);
                return context.SaveChanges();
            }
        }

        public int UpdateItemUsage(ItemUsage item)
        {
            using (var context = new ManagementSystemContext())
            {
                var existing = context.
                    ItemUsages.
                    Include(p => p.ItemUsageDetails).
                    SingleOrDefault(u => u.ItemUsageId == item.ItemUsageId);

                if (existing == null) return 0;

                // update allowed fields
                existing.ItemUsageDate = item.ItemUsageDate;
                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = item.ModifiedBy;

                existing.ItemUsageDetails.Where(d => !item.ItemUsageDetails.Any(nd => nd.ItemUsageDetailId == d.ItemUsageDetailId)).ToList().ForEach(d =>
                {
                    // remove details that are not in the updated item
                    context.ItemUsageDetails.Remove(d);
                });

                foreach (var detail in item.ItemUsageDetails)
                {
                    var existingDetail =
                        existing.
                        ItemUsageDetails.
                        SingleOrDefault(d => d.ItemUsageDetailId == detail.ItemUsageDetailId);

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
                        detail.ItemUsage = existing;
                        existing.ItemUsageDetails.Add(detail);
                    }
                }

                context.ItemUsages.Update(existing);

                return context.SaveChanges();
            }
        }

        public ItemUsage? DeleteItemUsage(int itemUsageId)
        {
            if (itemUsageId == 0) throw new ArgumentNullException(nameof(itemUsageId));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.ItemUsages.SingleOrDefault(u => u.ItemUsageId == itemUsageId);
                if (existing == null) goto ReturnResult;

                context.ItemUsages.Remove(existing);
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
