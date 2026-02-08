
namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class ItemUsageDetail
{
    private string _ItemName = "";
    public string ItemName
    {
        get { return _ItemName; }
        set
        {
            _ItemName = value;
            OnPropertyChanged("ItemName");
        }
    }

    public void Clear()
    {
        this.ItemUsageId = 0;
        this.ItemUsageDetailId = 0;
        this.ItemCode = "";
        this.Qty = 0;
        this.CreatedAt = DateTime.Now;
        this.CreatedBy = "";
        this.ModifiedAt = DateTime.Now;
        this.ModifiedBy = "";

        this.ItemName = "";
    }

    public void CopyFrom(ItemUsageDetail source)
    {
        this.ItemUsageId = source.ItemUsageId;
        this.ItemUsageDetailId = source.ItemUsageDetailId;
        this.ItemCode = source.ItemCode;
        this.Qty = source.Qty;
        this.CreatedAt = source.CreatedAt;
        this.CreatedBy = source.CreatedBy;
        this.ModifiedAt = source.ModifiedAt;
        this.ModifiedBy = source.ModifiedBy;
        this.ItemName = source.ItemName;
    }
}
