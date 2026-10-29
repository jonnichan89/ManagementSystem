
namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class PSOrderPT : INotifyPropertyChanged 
{
    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    // Helper method to trigger the event
    public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        AfterPropertyChanged(propertyName);
    }

    partial void AfterPropertyChanged(string propertyName);

    #endregion

    private int _PSId;
    
    public int PSId {
        get {
            return _PSId;
        }
        set {
            _PSId = value;
            OnPropertyChanged("PSId");
        }
    }

    private int _PSTId;
    
    public int PSTId {
        get {
            return _PSTId;
        }
        set {
            _PSTId = value;
            OnPropertyChanged("PSTId");
        }
    }

    private int _PTId;
    
    public int PTId {
        get {
            return _PTId;
        }
        set {
            _PTId = value;
            OnPropertyChanged("PTId");
        }
    }

    private string _PTName = string.Empty;
    
    public string PTName {
        get {
            return _PTName;
        }
        set {
            _PTName = value;
            OnPropertyChanged("PTName");
        }
    }

    private decimal _Qty;
    
    public decimal Qty {
        get {
            return _Qty;
        }
        set {
            _Qty = value;
            OnPropertyChanged("Qty");
        }
    }

    private string _Unit = string.Empty;
    
    public string Unit {
        get {
            return _Unit;
        }
        set {
            _Unit = value;
            OnPropertyChanged("Unit");
        }
    }

    private decimal _UnitPrice;
    
    public decimal UnitPrice {
        get {
            return _UnitPrice;
        }
        set {
            _UnitPrice = value;
            OnPropertyChanged("UnitPrice");
        }
    }

    private decimal _UnitPriceDiscount;
    
    public decimal UnitPriceDiscount {
        get {
            return _UnitPriceDiscount;
        }
        set {
            _UnitPriceDiscount = value;
            OnPropertyChanged("UnitPriceDiscount");
        }
    }

    private decimal _DPP;
    
    public decimal DPP {
        get {
            return _DPP;
        }
        set {
            _DPP = value;
            OnPropertyChanged("DPP");
        }
    }

    private decimal _DPPDiscount;
    
    public decimal DPPDiscount {
        get {
            return _DPPDiscount;
        }
        set {
            _DPPDiscount = value;
            OnPropertyChanged("DPPDiscount");
        }
    }

    private decimal _TotalAmount;
    
    public decimal TotalAmount {
        get {
            return _TotalAmount;
        }
        set {
            _TotalAmount = value;
            OnPropertyChanged("TotalAmount");
        }
    }

    private PSOrder _PSOrder = null!;
    
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual PSOrder PSOrder {
        get {
            return _PSOrder;
        }
        set {
            _PSOrder = value;
            OnPropertyChanged("PSOrder");
        }
    }
}
