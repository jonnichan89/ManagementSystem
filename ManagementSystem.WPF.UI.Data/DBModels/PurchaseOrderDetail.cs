
namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class PurchaseOrderDetail : INotifyPropertyChanged 
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

    private int _POId;
    
    public int POId {
        get {
            return _POId;
        }
        set {
            _POId = value;
            OnPropertyChanged("POId");
        }
    }

    private int _PODId;
    
    public int PODId {
        get {
            return _PODId;
        }
        set {
            _PODId = value;
            OnPropertyChanged("PODId");
        }
    }

    private string _ItemCode = string.Empty;
    
    public string ItemCode {
        get {
            return _ItemCode;
        }
        set {
            _ItemCode = value;
            OnPropertyChanged("ItemCode");
        }
    }

    private string _ItemName = string.Empty;
    
    public string ItemName {
        get {
            return _ItemName;
        }
        set {
            _ItemName = value;
            OnPropertyChanged("ItemName");
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

    private PurchaseOrder _PurchaseOrder = null!;
    
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual PurchaseOrder PurchaseOrder {
        get {
            return _PurchaseOrder;
        }
        set {
            _PurchaseOrder = value;
            OnPropertyChanged("PurchaseOrder");
        }
    }
}
