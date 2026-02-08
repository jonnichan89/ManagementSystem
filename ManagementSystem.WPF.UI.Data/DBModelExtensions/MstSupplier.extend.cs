

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class MstSupplier
{
    #region Search
    public string SearchKey { get { return SupplierCode + SupplierName; } }

    public bool PutSearchToTop
    {
        set
        {
            Askterisk = (value ? "*" : "");
        }
    }

    public string Askterisk { get; private set; } = "";

    #endregion

}
