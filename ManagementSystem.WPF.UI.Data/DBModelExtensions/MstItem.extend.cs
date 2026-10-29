

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class MstItem
{
    #region Search
    public string SearchKey { get { return ItemCode + ItemName; } }

    public bool PutSearchToTop
    {
        set
        {
            Askterisk = (value ? "*" : "");
        }
    }

    public string Askterisk { get; private set; } = "";

    #endregion

    private string _SupplierName = ""!;
    private string _ImagePathToUpload = ""!;

    public string ImagePathToUpload
    {
        get
        {
            return _ImagePathToUpload;
        }
        set
        {
            _ImagePathToUpload = value;
            OnPropertyChanged("ImagePathToUpload");
        }
    }

    public string SupplierName
    {
        get
        {
            return _SupplierName;
        }
        set
        {
            _SupplierName = value;
            OnPropertyChanged("SupplierName");
        }
    }

    public List<ItemCalc> ItemCalcs { get; set; } = null;
}
