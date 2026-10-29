using System;

namespace ManagementSystem.Data.Models;

public partial class PSOrderDetail
{
    public void SetDefault()
    {
        if (string.IsNullOrEmpty(ItemCode)) ItemCode = "";
        if (string.IsNullOrEmpty(ItemName)) ItemName = "";
        if (string.IsNullOrEmpty(Unit)) Unit = "";
    }

    public void CopyFrom(PSOrderDetail source)
    {
        this.ItemCode = source.ItemCode;
        this.ItemName = source.ItemName;
        this.Qty = source.Qty;
        this.Unit = source.Unit;
        this.UnitPrice = source.UnitPrice;
        this.UnitPriceDiscount = source.UnitPriceDiscount;
        this.DPP = source.DPP;
        this.DPPDiscount = source.DPPDiscount;
        this.TotalAmount = source.TotalAmount;
    }
}
