
namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class PurchaseOrderDetail
{
    public void Clear()
    {
        this.POId = 0;
        this.PODId = 0;
        this.ItemCode = "";
        this.ItemName = "";
        this.Qty = 0;
        this.Unit = "";
        this.UnitPrice = 0;
        this.UnitPriceDiscount = 0;
        this.DPP = 0;
        this.DPPDiscount = 0;
        this.TotalAmount = 0;
    }

    public void CopyFrom(PurchaseOrderDetail source)
    {
        this.POId = source.POId;
        this.PODId = source.PODId;
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
