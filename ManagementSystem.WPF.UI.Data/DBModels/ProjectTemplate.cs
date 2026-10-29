using System;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class ProjectTemplate : INotifyPropertyChanged 
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

    private byte? _PTNo;
    
    public byte? PTNo {
        get {
            return _PTNo;
        }
        set {
            _PTNo = value;
            OnPropertyChanged("PTNo");
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

    private string _ImagePath = string.Empty;
    
    public string ImagePath {
        get {
            return _ImagePath;
        }
        set {
            _ImagePath = value;
            OnPropertyChanged("ImagePath");
        }
    }

    private int _ImageFileSize;
    
    public int ImageFileSize {
        get {
            return _ImageFileSize;
        }
        set {
            _ImageFileSize = value;
            OnPropertyChanged("ImageFileSize");
        }
    }

    private int? _ParentPTId;
    
    public int? ParentPTId {
        get {
            return _ParentPTId;
        }
        set {
            _ParentPTId = value;
            OnPropertyChanged("ParentPTId");
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

    public virtual ObservableCollection<ProjectTemplate> ProjectTemplates { get; set; } = new ObservableCollection<ProjectTemplate>();

    private ProjectTemplate? _ParentProjectTemplate;
    
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ProjectTemplate? ParentProjectTemplate {
        get {
            return _ParentProjectTemplate;
        }
        set {
            _ParentProjectTemplate = value;
            OnPropertyChanged("ParentProjectTemplate");
        }
    }

    public virtual ObservableCollection<ProjectTemplateDetail> ProjectTemplateDetails { get; set; } = new ObservableCollection<ProjectTemplateDetail>();
}
