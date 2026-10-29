using System;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class ProjectTemplateDetail : INotifyPropertyChanged 
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

    private int _PTDId;
    
    public int PTDId {
        get {
            return _PTDId;
        }
        set {
            _PTDId = value;
            OnPropertyChanged("PTDId");
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

    private string _Note = string.Empty;
    
    public string Note {
        get {
            return _Note;
        }
        set {
            _Note = value;
            OnPropertyChanged("Note");
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

    private ProjectTemplate _ProjectTemplate = null!;
    
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ProjectTemplate ProjectTemplate {
        get {
            return _ProjectTemplate;
        }
        set {
            _ProjectTemplate = value;
            OnPropertyChanged("ProjectTemplate");
        }
    }
}
