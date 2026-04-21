using System;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class MstAccount : INotifyPropertyChanged 
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

    private int _AccountId;
    
    public int AccountId {
        get {
            return _AccountId;
        }
        set {
            _AccountId = value;
            OnPropertyChanged("AccountId");
        }
    }

    private string _AccountCode = string.Empty;
    
    public string AccountCode {
        get {
            return _AccountCode;
        }
        set {
            _AccountCode = value;
            OnPropertyChanged("AccountCode");
        }
    }

    private string _AccountName = string.Empty;
    
    public string AccountName {
        get {
            return _AccountName;
        }
        set {
            _AccountName = value;
            OnPropertyChanged("AccountName");
        }
    }

    private string _ModuleType = string.Empty;
    
    public string ModuleType {
        get {
            return _ModuleType;
        }
        set {
            _ModuleType = value;
            OnPropertyChanged("ModuleType");
        }
    }

    private string _DebitNameFormat = string.Empty;
    
    public string DebitNameFormat {
        get {
            return _DebitNameFormat;
        }
        set {
            _DebitNameFormat = value;
            OnPropertyChanged("DebitNameFormat");
        }
    }

    private string _CreditNameFormat = string.Empty;
    
    public string CreditNameFormat {
        get {
            return _CreditNameFormat;
        }
        set {
            _CreditNameFormat = value;
            OnPropertyChanged("CreditNameFormat");
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
