namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class MstAccount
{
    #region Search
    public string SearchKey { get { return AccountCode + AccountName; } }

    public bool PutSearchToTop
    {
        set
        {
            Askterisk = (value ? "*" : "");
        }
    }

    public string Askterisk { get; private set; } = "";

    #endregion

    public bool MTMenuPurchase {
        get {
            return _ModuleType.Contains("-PO");
        }
        set {
            if (!MTMenuPurchase)
            {
                _ModuleType = _ModuleType.
                    Replace("-PO", "").
                    Replace("-SO", "").
                    Replace("-OT", "") + "-PO";

                RaiseMTPropertyChanged();
            }
        }
    }

    public bool MTMenuSales
    {
        get
        {
            return _ModuleType.Contains("-SO");
        }
        set
        {
            if (!MTMenuSales)
            {
                _ModuleType = _ModuleType.
                    Replace("-PO", "").
                    Replace("-SO", "").
                    Replace("-OT", "") + "-SO";

                RaiseMTPropertyChanged();
            }
        }
    }

    public bool MTOther
    {
        get
        {
            return _ModuleType.Contains("-OT");
        }
        set
        {
            if (!MTOther)
            {
                _ModuleType = _ModuleType.
                    Replace("-PO", "").
                    Replace("-SO", "").
                    Replace("-OT", "") + "-OT";

                RaiseMTPropertyChanged();
            }
        }
    }

    public bool MTAPDebitNote
    {
        get
        {
            return _ModuleType.Contains("-TAPDN");
        }
        set
        {
            if (!MTAPDebitNote)
            {
                _ModuleType = _ModuleType.
                    Replace("-TAPDN", "").
                    Replace("-TAPCN", "").
                    Replace("-TARDN", "").
                    Replace("-TARCN", "") + "-TAPDN";

                RaiseMTPropertyChanged();
            }
        }
    }
    public bool MTAPCreditNote
    {
        get
        {
            return _ModuleType.Contains("-TAPCN");
        }
        set
        {
            if (!MTAPCreditNote)
            {
                _ModuleType = _ModuleType.
                    Replace("-TAPDN", "").
                    Replace("-TAPCN", "").
                    Replace("-TARDN", "").
                    Replace("-TARCN", "") + "-TAPCN";

                RaiseMTPropertyChanged();
            }
        }
    }
    public bool MTARDebitNote
    {
        get
        {
            return _ModuleType.Contains("-TARDN");
        }
        set
        {
            if (!MTARDebitNote)
            {
                _ModuleType = _ModuleType.
                    Replace("-TAPDN", "").
                    Replace("-TAPCN", "").
                    Replace("-TARDN", "").
                    Replace("-TARCN", "") + "-TARDN";

                RaiseMTPropertyChanged();
            }
        }
    }
    public bool MTARCreditNote
    {
        get
        {
            return _ModuleType.Contains("-TARCN");
        }
        set
        {
            if (!MTARCreditNote)
            {
                _ModuleType = _ModuleType.
                    Replace("-TAPDN", "").
                    Replace("-TAPCN", "").
                    Replace("-TARDN", "").
                    Replace("-TARCN", "") + "-TARCN";

                RaiseMTPropertyChanged();
            }
        }
    }

    private void RaiseMTPropertyChanged()
    {
        OnPropertyChanged("MTMenuPurchase");
        OnPropertyChanged("MTMenuSales");
        OnPropertyChanged("MTOther");
        OnPropertyChanged("MTAPDebitNote");
        OnPropertyChanged("MTAPCreditNote");
        OnPropertyChanged("MTARDebitNote");
        OnPropertyChanged("MTARCreditNote");
    }
}
