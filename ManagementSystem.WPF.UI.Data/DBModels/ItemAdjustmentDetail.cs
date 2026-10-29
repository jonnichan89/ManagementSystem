using System;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class ItemAdjustmentDetail : INotifyPropertyChanged 
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

    private int _IAId;
    
    public int IAId {
        get {
            return _IAId;
        }
        set {
            _IAId = value;
            OnPropertyChanged("IAId");
        }
    }

    private int _IADId;
    
    public int IADId {
        get {
            return _IADId;
        }
        set {
            _IADId = value;
            OnPropertyChanged("IADId");
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

    private DateTime _CreatedAt;
    
    public DateTime CreatedAt {
        get {
            return _CreatedAt;
        }
        set {
            _CreatedAt = value;
            OnPropertyChanged("CreatedAt");
        }
    }

    private string _CreatedBy = string.Empty;
    
    public string CreatedBy {
        get {
            return _CreatedBy;
        }
        set {
            _CreatedBy = value;
            OnPropertyChanged("CreatedBy");
        }
    }

    private DateTime _ModifiedAt;
    
    public DateTime ModifiedAt {
        get {
            return _ModifiedAt;
        }
        set {
            _ModifiedAt = value;
            OnPropertyChanged("ModifiedAt");
        }
    }

    private string _ModifiedBy = string.Empty;
    
    public string ModifiedBy {
        get {
            return _ModifiedBy;
        }
        set {
            _ModifiedBy = value;
            OnPropertyChanged("ModifiedBy");
        }
    }

    private ItemAdjustment _ItemAdjustment = null!;
    
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ItemAdjustment ItemAdjustment {
        get {
            return _ItemAdjustment;
        }
        set {
            _ItemAdjustment = value;
            OnPropertyChanged("ItemAdjustment");
        }
    }
}
