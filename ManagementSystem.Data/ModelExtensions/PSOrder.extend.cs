using System;

namespace ManagementSystem.Data.Models;

public partial class PSOrder
{
    public void SetDefault()
    {
        if (string.IsNullOrEmpty(PSCode)) PSCode = "";
        if (string.IsNullOrEmpty(SupplierCode)) SupplierCode = "";
        if (string.IsNullOrEmpty(SupplierName)) SupplierName = "";
        if (string.IsNullOrEmpty(CurrencyId)) CurrencyId = "";
        if (string.IsNullOrEmpty(PaidCurrencyId)) PaidCurrencyId = "";
        if (string.IsNullOrEmpty(PaidNote)) PaidNote = "";
        if (string.IsNullOrEmpty(CreatedBy)) CreatedBy = "";
        if (string.IsNullOrEmpty(ModifiedBy)) ModifiedBy = "";

        foreach (PSOrderDetail item in PSOrderDetails)
        {
            item.SetDefault();
        }
    }
}
