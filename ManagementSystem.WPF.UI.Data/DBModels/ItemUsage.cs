using System;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class ItemUsage : INotifyPropertyChanged 
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

    private DateTime _ItemUsageDate;
    
    public DateTime ItemUsageDate {
        get {
            return _ItemUsageDate;
        }
        set {
            _ItemUsageDate = value;
            OnPropertyChanged("ItemUsageDate");
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

    public virtual ObservableCollection<ItemUsageDetail> ItemUsageDetails { get; set; } = new ObservableCollection<ItemUsageDetail>();
}
