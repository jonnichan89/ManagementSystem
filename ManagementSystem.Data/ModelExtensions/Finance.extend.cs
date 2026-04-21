using System;

namespace ManagementSystem.Data.Models;

public partial class Finance
{
    public void SetDefault()
    {
        //[FinanceType], [ReferenceType], [ReferenceCode]
        //, [AccountCode], [AccountName], [SupplierCode], [SupplierName]
        //, [SupplierReferenceNo], [CurrencyId]
        //, [PaidCurrencyId], [PaidNote], [CreatedBy], [ModifiedAt], [ModifiedBy]
        if (string.IsNullOrEmpty(FinanceCode)) FinanceCode = "";
        if (string.IsNullOrEmpty(ReferenceType)) ReferenceType = "";
        if (string.IsNullOrEmpty(ReferenceCode)) ReferenceCode = "";
        if (string.IsNullOrEmpty(AccountCode)) AccountCode = "";
        if (string.IsNullOrEmpty(AccountName)) AccountName = "";
        if (string.IsNullOrEmpty(SupplierCode)) SupplierCode = "";
        if (string.IsNullOrEmpty(SupplierName)) SupplierName = "";
        if (string.IsNullOrEmpty(SupplierReferenceNo)) SupplierReferenceNo = "";
        if (string.IsNullOrEmpty(CurrencyId)) CurrencyId = "";
        if (string.IsNullOrEmpty(PaidCurrencyId)) PaidCurrencyId = "";
        if (string.IsNullOrEmpty(PaidNote)) PaidNote = "";
        if (string.IsNullOrEmpty(CreatedBy)) CreatedBy = "";
        if (string.IsNullOrEmpty(ModifiedBy)) ModifiedBy = "";

        foreach (FinanceDetail item in FinanceDetails)
        {
            item.SetDefault();
        }
    }
}
