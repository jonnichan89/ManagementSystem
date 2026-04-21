using System;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class MstUser : INotifyPropertyChanged 
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

    private string _UserId = string.Empty;
    
    public string UserId {
        get {
            return _UserId;
        }
        set {
            _UserId = value;
            OnPropertyChanged("UserId");
        }
    }

    private string _UserName = string.Empty;
    
    public string UserName {
        get {
            return _UserName;
        }
        set {
            _UserName = value;
            OnPropertyChanged("UserName");
        }
    }

    private string _UserPassword = string.Empty;
    
    public string UserPassword {
        get {
            return _UserPassword;
        }
        set {
            _UserPassword = value;
            OnPropertyChanged("UserPassword");
        }
    }

    private string _Salt = string.Empty;
    
    public string Salt {
        get {
            return _Salt;
        }
        set {
            _Salt = value;
            OnPropertyChanged("Salt");
        }
    }

    private string _AccessList = string.Empty;
    
    public string AccessList {
        get {
            return _AccessList;
        }
        set {
            _AccessList = value;
            OnPropertyChanged("AccessList");
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
