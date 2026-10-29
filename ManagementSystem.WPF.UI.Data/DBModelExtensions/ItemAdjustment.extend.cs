
namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class ItemAdjustment
{
    public void AddItemAdjustmentDetail()
    {
        ItemAdjustmentDetails.Add(new ItemAdjustmentDetail()
        {
            ItemName = "",
            ItemCode = "",
            Qty = 0,
            CreatedAt = DateTime.Now,
            CreatedBy = "",
            ModifiedAt = DateTime.Now,
            ModifiedBy = "",
            ItemAdjustment = this
        });

    }
}
