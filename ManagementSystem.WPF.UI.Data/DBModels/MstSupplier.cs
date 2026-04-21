using System;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class MstSupplier : INotifyPropertyChanged 
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

    private int _SupplierId;
    
    public int SupplierId {
        get {
            return _SupplierId;
        }
        set {
            _SupplierId = value;
            OnPropertyChanged("SupplierId");
        }
    }

    private string _SupplierCode = string.Empty;
    
    public string SupplierCode {
        get {
            return _SupplierCode;
        }
        set {
            _SupplierCode = value;
            OnPropertyChanged("SupplierCode");
        }
    }

    private string _SupplierName = string.Empty;
    
    public string SupplierName {
        get {
            return _SupplierName;
        }
        set {
            _SupplierName = value;
            OnPropertyChanged("SupplierName");
        }
    }

    private byte _SupplierType;
    
    public byte SupplierType {
        get {
            return _SupplierType;
        }
        set {
            _SupplierType = value;
            OnPropertyChanged("SupplierType");
        }
    }

    private bool _IsBlacklist;
    
    public bool IsBlacklist {
        get {
            return _IsBlacklist;
        }
        set {
            _IsBlacklist = value;
            OnPropertyChanged("IsBlacklist");
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
}
