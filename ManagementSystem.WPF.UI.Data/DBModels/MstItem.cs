using System;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class MstItem : INotifyPropertyChanged 
{
    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    // Helper method to trigger the event
    public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    private int _ItemId;
    
    public int ItemId {
        get {
            return _ItemId;
        }
        set {
            _ItemId = value;
            OnPropertyChanged("ItemId");
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

    private string _ItemName = null!;
    
    public string ItemName {
        get {
            return _ItemName;
        }
        set {
            _ItemName = value;
            OnPropertyChanged("ItemName");
        }
    }

    private string _ItemDescription = null!;
    
    public string ItemDescription {
        get {
            return _ItemDescription;
        }
        set {
            _ItemDescription = value;
            OnPropertyChanged("ItemDescription");
        }
    }

    private string _ItemUnit = null!;
    
    public string ItemUnit {
        get {
            return _ItemUnit;
        }
        set {
            _ItemUnit = value;
            OnPropertyChanged("ItemUnit");
        }
    }

    private string _ImagePath = null!;
    
    public string ImagePath {
        get {
            return _ImagePath;
        }
        set {
            _ImagePath = value;
            OnPropertyChanged("ImagePath");
        }
    }

    private bool _IsStockControl;
    
    public bool IsStockControl {
        get {
            return _IsStockControl;
        }
        set {
            _IsStockControl = value;
            OnPropertyChanged("IsStockControl");
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

    private int _MinimumStock;
    
    public int MinimumStock {
        get {
            return _MinimumStock;
        }
        set {
            _MinimumStock = value;
            OnPropertyChanged("MinimumStock");
        }
    }

    private string _SupplierCode = null!;
    
    public string SupplierCode {
        get {
            return _SupplierCode;
        }
        set {
            _SupplierCode = value;
            OnPropertyChanged("SupplierCode");
        }
    }
}
