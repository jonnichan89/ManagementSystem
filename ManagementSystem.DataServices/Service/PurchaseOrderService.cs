
namespace ManagementSystem.DataServices.Service
{
    public class PurchaseOrderService
    {
        //SharedResources _resShared;
        //public PurchaseOrderService(SharedResources resShared) { }
        public PurchaseOrderService() { }

        public List<ModelValidation> CanContinueAction(ServiceAction action, PurchaseOrder model)
        {
            List<ModelValidation> validations = new List<ModelValidation>();

            model.PurchaseOrderDetails.GroupBy(d => d.ItemCode).ToList().ForEach(g =>
            {
                if (g.Count() > 1)
                {
                    validations.Add(ModelValidation.DuplicateRecord(
                        nameof(PurchaseOrderDetail),
                        nameof(PurchaseOrderDetail.ItemCode),
                        g.Key));
                }
            });

            // Validasi taro dibawah karena Delete cuma terisi PK nya saja
            if (action != ServiceAction.Delete)
            {
            }

            return validations;
        }

        public IEnumerable<PurchaseOrder> GetPurchaseOrders()
        {
            using (var context = new ManagementSystemContext())
            {
                var items = context.
                    PurchaseOrders.
                    Include(p => p.PurchaseOrderDetails).
                    ToList();
                return items;
            }
        }

        public bool IsExists(ServiceAction action, PurchaseOrder model)
        {
            //if (action == ServiceAction.Add || action == ServiceAction.Get)
            //    using (var context = new ManagementSystemContext())
            //    {
            //        return context.
            //            PurchaseOrders.
            //            Include(p => p.PurchaseOrderDetails).
            //            Any(u => u.POId != model.POId && u.PurchaseOrderDate == model.PurchaseOrderDate);
            //    }

            return false;
        }

        public bool IsExists(ServiceAction action, PurchaseOrderDetail model)
        {
            //using (var context = new ManagementSystemContext())
            //{
            //    return context.
            //        PurchaseOrderDetails.
            //        Any(u =>
            //            u.PurchaseOrderId == model.PurchaseOrderId &&
            //            u.PurchaseOrderDetailId != model.PurchaseOrderDetailId &&
            //            u.ItemCode == model.ItemCode);
            //}
            return false;
        }

        public PurchaseOrder? GetPurchaseOrderById(int POId)
        {
            if (POId == 0) throw new ArgumentNullException(nameof(POId));

            using (var context = new ManagementSystemContext())
            {
                PurchaseOrder? item = context.
                    PurchaseOrders.
                    Include(p => p.PurchaseOrderDetails).
                    SingleOrDefault(u => u.POId == POId);
                return item;
            }
        }

        public int CreatePurchaseOrder(PurchaseOrder item)
        {
            using (var context = new ManagementSystemContext())
            {
                var now = DateTime.Now;
                item.CreatedAt = now;
                item.ModifiedAt = now;

                foreach (var detail in item.PurchaseOrderDetails)
                {
                    detail.PurchaseOrder = item;
                }

                context.PurchaseOrders.Add(item);
                return context.SaveChanges();
            }
        }

        public int UpdatePurchaseOrder(PurchaseOrder item)
        {
            using (var context = new ManagementSystemContext())
            {
                var existing = context.
                    PurchaseOrders.
                    Include(p => p.PurchaseOrderDetails).
                    SingleOrDefault(u => u.POId == item.POId);

                if (existing == null) return 0;

                // update allowed fields
                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = item.ModifiedBy;

                existing.PurchaseOrderDetails.Where(d => !item.PurchaseOrderDetails.Any(nd => nd.PODId == d.PODId)).ToList().ForEach(d =>
                {
                    // remove details that are not in the updated item
                    context.PurchaseOrderDetails.Remove(d);
                });

                foreach (var detail in item.PurchaseOrderDetails)
                {
                    var existingDetail =
                        existing.
                        PurchaseOrderDetails.
                        SingleOrDefault(d => d.PODId == detail.PODId);

                    if (existingDetail != null)
                    {
                        // update allowed fields
                        existingDetail.ItemCode = detail.ItemCode;
                        existingDetail.Qty = detail.Qty;
                        //existingDetail.ModifiedAt = DateTime.Now;
                        //existingDetail.ModifiedBy = detail.ModifiedBy;
                    }
                    else
                    {
                        // new detail
                        //detail.CreatedAt = DateTime.Now;
                        //detail.ModifiedAt = DateTime.Now;
                        detail.PurchaseOrder = existing;
                        existing.PurchaseOrderDetails.Add(detail);
                    }
                }

                context.PurchaseOrders.Update(existing);

                return context.SaveChanges();
            }
        }

        public PurchaseOrder? DeletePurchaseOrder(int POId)
        {
            if (POId == 0) throw new ArgumentNullException(nameof(POId));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.PurchaseOrders.SingleOrDefault(u => u.POId == POId);
                if (existing == null) goto ReturnResult;

                context.PurchaseOrders.Remove(existing);
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
