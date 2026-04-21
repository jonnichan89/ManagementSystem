using ManagementSystem.Common.OtherModels;

namespace ManagementSystem.DataServices.Service
{
    public class SalesOrderService
    {
        //SharedResources _resShared;
        //public SalesOrderService(SharedResources resShared) { }
        public SalesOrderService() { }

        public List<ModelValidation> CanContinueAction(ServiceAction action, PSOrder model)
        {
            List<ModelValidation> validations = new List<ModelValidation>();

            if (action == ServiceAction.Delete)
            {
                if (model.PSId <= 0)
                    validations.Add(ModelValidation.FieldIsRequired(nameof(PSOrder), nameof(model.PSId), model.PSId.ToString()));
            }
            else
            {
                model.PSOrderDetails.GroupBy(d => d.ItemCode).ToList().ForEach(g =>
                {
                    if (g.Count() > 1)
                    {
                        validations.Add(ModelValidation.DuplicateRecord(
                            nameof(PSOrderDetail),
                            nameof(PSOrderDetail.ItemCode),
                            g.Key));
                    }
                });
            }

            return validations;
        }

        public IEnumerable<PSOrder> GetSalesOrders()
        {
            using (var context = new ManagementSystemContext())
            {
                var items = context.
                    PSOrders.
                    Include(p => p.PSOrderDetails).
                    Where(p => p.PSType == "SO").
                    ToList();
                return items;
            }
        }

        public IEnumerable<SearchOptionResultPSOrder> SearchSalesOrdersForOptions(SearchPSOrderForOptions searchPS)
        {
            if (searchPS == null) throw new ArgumentNullException(nameof(searchPS));

            using (var context = new ManagementSystemContext())
            {
                SearchOptionResultPSOrder[] item = context.
                    PSOrders.
                    Include(p => p.PSOrderDetails).
                    Where(p => p.PSType == "SO" && 
                        p.PSCode.Contains(searchPS.PSCode)).
                    Select(p => new SearchOptionResultPSOrder() {
                        PSCode = p.PSCode,
                        SupplierCode = p.SupplierCode,
                        SupplierName = p.SupplierName,
                    }).ToArray();
                return item;
            }
        }

        public bool IsExists(ServiceAction action, PSOrder model)
        {
            //if (action == ServiceAction.Add || action == ServiceAction.Get)
            //    using (var context = new ManagementSystemContext())
            //    {
            //        return context.
            //            PSOrders.
            //            Include(p => p.PSOrderDetails).
            //            Any(u => u.PSId != model.PSId && u.PSOrderDate == model.PSOrderDate);
            //    }

            return false;
        }

        public bool IsExists(ServiceAction action, PSOrderDetail model)
        {
            //using (var context = new ManagementSystemContext())
            //{
            //    return context.
            //        PSOrderDetails.
            //        Any(u =>
            //            u.PSOrderId == model.PSOrderId &&
            //            u.PSOrderDetailId != model.PSOrderDetailId &&
            //            u.ItemCode == model.ItemCode);
            //}
            return false;
        }

        public PSOrder? GetSalesOrderById(int PSId)
        {
            if (PSId == 0) throw new ArgumentNullException(nameof(PSId));

            using (var context = new ManagementSystemContext())
            {
                PSOrder? item = context.
                    PSOrders.
                    Include(p => p.PSOrderDetails).
                    Where(p => p.PSType == "SO").
                    SingleOrDefault(u => u.PSId == PSId);
                return item;
            }
        }

        public int CreateSalesOrder(PSOrder item)
        {
            using (var context = new ManagementSystemContext())
            {
                var now = DateTime.Now;
                item.CreatedAt = now;
                item.ModifiedAt = now;

                // Get Last SO Code
                string prePSCode = $@"SO-{item.PSDate.ToString("ddMMyyyy")}-001";
                string newPSCode = "";
                string lastPSCode = context.
                    PSOrders.
                    Where(p => p.PSType == "SO" && p.PSCode.StartsWith(prePSCode)).Max(p => p.PSCode);
                int lastPSNumber = 0;
                if (!string.IsNullOrWhiteSpace(lastPSCode))
                {
                    if (!int.TryParse(lastPSCode.Replace(prePSCode, ""), out lastPSNumber))
                    {
                        lastPSNumber = 0;
                    }
                }

                // Create new SO Code
                lastPSNumber += 1;
                newPSCode = prePSCode + lastPSNumber.ToString().PadLeft(3, '0');
                while (context.
                    PSOrders.
                    Where(p => p.PSType == "SO").
                    Count(p => p.PSCode == newPSCode) > 0)
                {
                    lastPSNumber++;
                    newPSCode = prePSCode + lastPSNumber.ToString().PadLeft(3, '0');
                }

                // Assign Children
                item.PSCode = newPSCode;
                foreach (var detail in item.PSOrderDetails)
                {
                    detail.PSOrder = item;
                }

                // Insert
                context.PSOrders.Add(item);
                return context.SaveChanges();
            }
        }

        public int UpdateSalesOrder(PSOrder item)
        {
            using (var context = new ManagementSystemContext())
            {
                var existing = context.
                    PSOrders.
                    Include(p => p.PSOrderDetails).
                    Where(p => p.PSType == "SO").
                    SingleOrDefault(u => u.PSId == item.PSId);

                if (existing == null) return 0;

                // update allowed fields
                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = item.ModifiedBy;

                existing.PSOrderDetails.Where(d => !item.PSOrderDetails.Any(nd => nd.PSDId == d.PSDId)).ToList().ForEach(d =>
                {
                    // remove details that are not in the updated item
                    context.PSOrderDetails.Remove(d);
                });

                foreach (var detail in item.PSOrderDetails)
                {
                    var existingDetail =
                        existing.
                        PSOrderDetails.
                        SingleOrDefault(d => d.PSDId == detail.PSDId);

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
                        detail.PSOrder = existing;
                        existing.PSOrderDetails.Add(detail);
                    }
                }

                context.PSOrders.Update(existing);

                return context.SaveChanges();
            }
        }

        public PSOrder? DeleteSalesOrder(int PSId)
        {
            if (PSId == 0) throw new ArgumentNullException(nameof(PSId));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.
                    PSOrders.
                    Where(p => p.PSType == "SO").
                    SingleOrDefault(u => u.PSId == PSId);
                if (existing == null) goto ReturnResult;

                context.PSOrders.Remove(existing);
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
