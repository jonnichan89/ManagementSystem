using System;

namespace ManagementSystem.Data.Models;

public partial class FinanceDetail
{
    public void SetDefault()
    {
        if (string.IsNullOrEmpty(PaidCurrencyId)) PaidCurrencyId = "";
        if (string.IsNullOrEmpty(PaidNote)) PaidNote = "";
        if (string.IsNullOrEmpty(CreatedBy)) CreatedBy = "";
        if (string.IsNullOrEmpty(ModifiedBy)) ModifiedBy = "";
    }
}
