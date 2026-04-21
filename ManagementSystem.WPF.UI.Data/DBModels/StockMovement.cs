using System;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class StockMovement : INotifyPropertyChanged 
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

    private int _SMId;
    
    public int SMId {
        get {
            return _SMId;
        }
        set {
            _SMId = value;
            OnPropertyChanged("SMId");
        }
    }

    private DateTime _SMDate;
    
    public DateTime SMDate {
        get {
            return _SMDate;
        }
        set {
            _SMDate = value;
            OnPropertyChanged("SMDate");
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

    private decimal _POQty;
    
    public decimal POQty {
        get {
            return _POQty;
        }
        set {
            _POQty = value;
            OnPropertyChanged("POQty");
        }
    }

    private decimal _POUsedQty;
    
    public decimal POUsedQty {
        get {
            return _POUsedQty;
        }
        set {
            _POUsedQty = value;
            OnPropertyChanged("POUsedQty");
        }
    }

    private int _SOId;
    
    public int SOId {
        get {
            return _SOId;
        }
        set {
            _SOId = value;
            OnPropertyChanged("SOId");
        }
    }

    private decimal _SOQty;
    
    public decimal SOQty {
        get {
            return _SOQty;
        }
        set {
            _SOQty = value;
            OnPropertyChanged("SOQty");
        }
    }

    private decimal _SOUsedQty;
    
    public decimal SOUsedQty {
        get {
            return _SOUsedQty;
        }
        set {
            _SOUsedQty = value;
            OnPropertyChanged("SOUsedQty");
        }
    }

    private decimal _SOUseSMId;
    
    public decimal SOUseSMId {
        get {
            return _SOUseSMId;
        }
        set {
            _SOUseSMId = value;
            OnPropertyChanged("SOUseSMId");
        }
    }

    private decimal _SOUseSMPOQty;
    
    public decimal SOUseSMPOQty {
        get {
            return _SOUseSMPOQty;
        }
        set {
            _SOUseSMPOQty = value;
            OnPropertyChanged("SOUseSMPOQty");
        }
    }

    private DateTime _LastModifiedAt;
    
    public DateTime LastModifiedAt {
        get {
            return _LastModifiedAt;
        }
        set {
            _LastModifiedAt = value;
            OnPropertyChanged("LastModifiedAt");
        }
    }
}
