
namespace ManagementSystem.DataServices.Service
{
    public class FinanceService
    {
        //SharedResources _resShared;
        //public FinanceService(SharedResources resShared) { }
        public FinanceService() { }

        public List<ModelValidation> CanContinueAction(ServiceAction action, Finance model)
        {
            List<ModelValidation> validations = new List<ModelValidation>();

            //model.FinanceDetails.GroupBy(d => d.ItemCode).ToList().ForEach(g =>
            //{
            //    if (g.Count() > 1)
            //    {
            //        validations.Add(ModelValidation.DuplicateRecord(
            //            nameof(FinanceDetail),
            //            nameof(FinanceDetail.ItemCode),
            //            g.Key));
            //    }
            //});

            if (action == ServiceAction.Delete)
            {
                if (model.FinanceId <= 0)
                    validations.Add(ModelValidation.FieldIsRequired(nameof(Finance), nameof(model.FinanceId), model.FinanceId.ToString()));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.ReferenceCode))
                    validations.Add(ModelValidation.FieldIsRequired(nameof(Finance), nameof(model.ReferenceCode), model.ReferenceCode));

                if (string.IsNullOrWhiteSpace(model.SupplierCode))
                    validations.Add(ModelValidation.FieldIsRequired(nameof(Finance), nameof(model.SupplierCode), model.SupplierCode));

                if (string.IsNullOrWhiteSpace(model.SupplierName))
                    validations.Add(ModelValidation.FieldIsRequired(nameof(Finance), nameof(model.SupplierName), model.SupplierName));
            }

            return validations;
        }

        public IEnumerable<Finance> GetFinances()
        {
            using (var context = new ManagementSystemContext())
            {
                var items = context.
                    Finances.
                    Include(p => p.FinanceDetails).
                    ToList();
                return items;
            }
        }

        public bool IsExists(ServiceAction action, Finance model)
        {
            //if (action == ServiceAction.Add || action == ServiceAction.Get)
            //    using (var context = new ManagementSystemContext())
            //    {
            //        return context.
            //            Finances.
            //            Include(p => p.FinanceDetails).
            //            Any(u => u.FinanceId != model.FinanceId && u.FinanceDate == model.FinanceDate);
            //    }

            return false;
        }

        public bool IsExists(ServiceAction action, FinanceDetail model)
        {
            //using (var context = new ManagementSystemContext())
            //{
            //    return context.
            //        FinanceDetails.
            //        Any(u =>
            //            u.FinanceId == model.FinanceId &&
            //            u.FinanceDetailId != model.FinanceDetailId &&
            //            u.ItemCode == model.ItemCode);
            //}
            return false;
        }

        public Finance? GetFinanceById(int financeId)
        {
            if (financeId == 0) throw new ArgumentNullException(nameof(financeId));

            using (var context = new ManagementSystemContext())
            {
                Finance? item = context.
                    Finances.
                    Include(p => p.FinanceDetails).
                    SingleOrDefault(u => u.FinanceId == financeId);
                return item;
            }
        }

        public int CreateFinance(Finance item)
        {
            using (var context = new ManagementSystemContext())
            {
                var now = DateTime.Now;
                item.CreatedAt = now;
                item.ModifiedAt = now;

                // Get Last Finance Code
                string preFinanceCode = $@"{item.ReferenceType}-{item.FinanceDate.ToString("ddMMyyyy")}-";
                string newFinanceCode = "";
                string lastFinanceCode = context.Finances.Where(p => p.FinanceCode.StartsWith(preFinanceCode)).Max(p => p.FinanceCode);
                int lastFinanceNumber = 0;
                if (!string.IsNullOrWhiteSpace(lastFinanceCode))
                {
                    if (!int.TryParse(lastFinanceCode.Replace(preFinanceCode, ""), out lastFinanceNumber))
                    {
                        lastFinanceNumber = 0;
                    }
                }

                // Create new Finance Code
                lastFinanceNumber += 1;
                newFinanceCode = preFinanceCode + lastFinanceNumber.ToString().PadLeft(3, '0');
                while (context.Finances.Count(p => p.FinanceCode == newFinanceCode) > 0)
                {
                    lastFinanceNumber++;
                    newFinanceCode = preFinanceCode + lastFinanceNumber.ToString().PadLeft(3, '0');
                }

                // Assign Children
                item.FinanceCode = newFinanceCode;
                foreach (var detail in item.FinanceDetails)
                {
                    detail.Finance = item;
                }

                item.IsPaid = (item.FinanceDetails.Count > 0);
                if (item.IsPaid)
                {
                    item.PaidAt = item.FinanceDetails.Max(p => p.PaidAt);
                    item.PaidCurrencyId = item.FinanceDetails.Last().PaidCurrencyId;
                    item.PaidCurrencyRate = item.FinanceDetails.Last().PaidCurrencyRate;
                    item.PaidAmount = item.FinanceDetails.Sum(p => p.PaidAmount);
                    item.PaidNote = "";
                }
                else
                {
                    item.PaidAt = null;
                    item.PaidCurrencyId = "";
                    item.PaidCurrencyRate = 0;
                    item.PaidAmount = 0;
                    item.PaidNote = "";
                }

                // Insert
                context.Finances.Add(item);
                return context.SaveChanges();
            }
        }

        public int UpdateFinance(Finance item)
        {
            using (var context = new ManagementSystemContext())
            {
                var existing = context.
                    Finances.
                    Include(p => p.FinanceDetails).
                    SingleOrDefault(u => u.FinanceId == item.FinanceId);

                if (existing == null) return 0;

                // update allowed fields
                existing.FinanceDate = item.FinanceDate;
                existing.FinanceType = item.FinanceType;
                existing.ReferenceType = item.ReferenceType;
                existing.ReferenceCode = item.ReferenceCode;
                existing.AccountCode = item.AccountCode;
                existing.AccountName = item.AccountName;
                existing.SupplierCode = item.SupplierCode;
                existing.SupplierName = item.SupplierName;
                existing.SupplierReferenceNo = item.SupplierReferenceNo;
                existing.CurrencyId = item.CurrencyId;
                existing.CurrencyRate = item.CurrencyRate;
                existing.PPNPercentage = item.PPNPercentage;
                existing.PPHPercentage = item.PPHPercentage;
                existing.DPP = item.DPP;
                existing.DPPDiscount = item.DPPDiscount;
                existing.PPN = item.PPN;
                existing.PPH = item.PPH;
                existing.TotalAmount = item.TotalAmount;
                existing.ModifiedAt = DateTime.Now;
                existing.ModifiedBy = item.ModifiedBy;

                existing.FinanceDetails.Where(d => !item.FinanceDetails.Any(nd => nd.FinanceDetailId == d.FinanceDetailId)).ToList().ForEach(d =>
                {
                    // remove details that are not in the updated item
                    context.FinanceDetails.Remove(d);
                });

                foreach (var detail in item.FinanceDetails)
                {
                    var existingDetail =
                        existing.
                        FinanceDetails.
                        SingleOrDefault(d => d.FinanceDetailId == detail.FinanceDetailId);

                    if (existingDetail != null)
                    {
                        // update allowed fields
                        existingDetail.PaidAt = detail.PaidAt;
                        existingDetail.PaidCurrencyId = detail.PaidCurrencyId;
                        existingDetail.PaidCurrencyRate = detail.PaidCurrencyRate;
                        existingDetail.PaidAmount = detail.PaidAmount;
                        existingDetail.PaidNote = detail.PaidNote;

                        existingDetail.ModifiedAt = DateTime.Now;
                        existingDetail.ModifiedBy = detail.ModifiedBy;
                    }
                    else
                    {
                        // new detail
                        detail.CreatedAt = DateTime.Now;
                        detail.ModifiedAt = DateTime.Now;
                        detail.Finance = existing;
                        existing.FinanceDetails.Add(detail);
                    }
                }

                existing.IsPaid = (existing.FinanceDetails.Count > 0);
                if (existing.IsPaid)
                {
                    existing.PaidAt = existing.FinanceDetails.Max(p => p.PaidAt);
                    existing.PaidCurrencyId = existing.FinanceDetails.Last().PaidCurrencyId;
                    existing.PaidCurrencyRate = existing.FinanceDetails.Last().PaidCurrencyRate;
                    existing.PaidAmount = existing.FinanceDetails.Sum(p => p.PaidAmount);
                    existing.PaidNote = "";
                }
                else
                {
                    existing.PaidAt = null;
                    existing.PaidCurrencyId = "";
                    existing.PaidCurrencyRate = 0;
                    existing.PaidAmount = 0;
                    existing.PaidNote = "";
                }

                context.Finances.Update(existing);

                return context.SaveChanges();
            }
        }

        public Finance? DeleteFinance(int financeId)
        {
            if (financeId == 0) throw new ArgumentNullException(nameof(financeId));

            using (var context = new ManagementSystemContext())
            {
                var existing = context.Finances.SingleOrDefault(u => u.FinanceId == financeId);
                if (existing == null) goto ReturnResult;

                context.Finances.Remove(existing);
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
