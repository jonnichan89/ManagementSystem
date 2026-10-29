
namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class ItemAdjustmentDetail
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
        this.IAId = 0;
        this.IADId = 0;
        this.ItemCode = "";
        this.Qty = 0;
        this.CreatedAt = DateTime.Now;
        this.CreatedBy = "";
        this.ModifiedAt = DateTime.Now;
        this.ModifiedBy = "";

        this.ItemName = "";
    }

    public void CopyFrom(ItemAdjustmentDetail source)
    {
        this.IAId = source.IAId;
        this.IADId = source.IADId;
        this.ItemCode = source.ItemCode;
        this.Qty = source.Qty;
        this.CreatedAt = source.CreatedAt;
        this.CreatedBy = source.CreatedBy;
        this.ModifiedAt = source.ModifiedAt;
        this.ModifiedBy = source.ModifiedBy;
        this.ItemName = source.ItemName;
    }
}
