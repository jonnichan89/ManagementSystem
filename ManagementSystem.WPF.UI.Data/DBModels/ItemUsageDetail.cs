using System;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class ItemUsageDetail : INotifyPropertyChanged 
{
    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    // Helper method to trigger the event
    public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    private int _ItemUsageId;
    
    public int ItemUsageId {
        get {
            return _ItemUsageId;
        }
        set {
            _ItemUsageId = value;
            OnPropertyChanged("ItemUsageId");
        }
    }

    private int _ItemUsageDetailId;
    
    public int ItemUsageDetailId {
        get {
            return _ItemUsageDetailId;
        }
        set {
            _ItemUsageDetailId = value;
            OnPropertyChanged("ItemUsageDetailId");
        }
    }

    private string _ItemCode = null!;
    
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

    private string _CreatedBy = null!;
    
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

    private string _ModifiedBy = null!;
    
    public string ModifiedBy {
        get {
            return _ModifiedBy;
        }
        set {
            _ModifiedBy = value;
            OnPropertyChanged("ModifiedBy");
        }
    }

    private ItemUsage _ItemUsage = null!;
    
    public virtual ItemUsage ItemUsage {
        get {
            return _ItemUsage;
        }
        set {
            _ItemUsage = value;
            OnPropertyChanged("ItemUsage");
        }
    }
}
