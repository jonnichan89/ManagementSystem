
namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class ItemUsage
{
    public void AddItemUsageDetail()
    {
        ItemUsageDetails.Add(new ItemUsageDetail()
        {
            ItemName = "",
            ItemCode = "",
            Qty = 0,
            CreatedAt = DateTime.Now,
            CreatedBy = "",
            ModifiedAt = DateTime.Now,
            ModifiedBy = "",
            ItemUsage = this
        });

    }
}
