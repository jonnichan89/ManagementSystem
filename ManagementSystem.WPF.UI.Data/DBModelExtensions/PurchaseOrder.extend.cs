
namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class PurchaseOrder
{
    public void AddPurchaseOrderDetail()
    {
        PurchaseOrderDetails.Add(new PurchaseOrderDetail()
        {
            POId = 0,
            PODId = 0,
            ItemCode = "",
            ItemName = "",
            Qty = 0,
            Unit = "",
            UnitPrice = 0,
            UnitPriceDiscount = 0,
            DPP = 0,
            DPPDiscount = 0,
            TotalAmount = 0,
            PurchaseOrder = this
        });

    }
}
