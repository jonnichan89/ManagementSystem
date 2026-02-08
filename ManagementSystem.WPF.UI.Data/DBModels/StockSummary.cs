using System;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class StockSummary : INotifyPropertyChanged 
{
    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    // Helper method to trigger the event
    public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    private DateTime _SSDate;
    
    public DateTime SSDate {
        get {
            return _SSDate;
        }
        set {
            _SSDate = value;
            OnPropertyChanged("SSDate");
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

    private decimal _BeginQty;
    
    public decimal BeginQty {
        get {
            return _BeginQty;
        }
        set {
            _BeginQty = value;
            OnPropertyChanged("BeginQty");
        }
    }

    private decimal _InQty;
    
    public decimal InQty {
        get {
            return _InQty;
        }
        set {
            _InQty = value;
            OnPropertyChanged("InQty");
        }
    }

    private decimal _OutQty;
    
    public decimal OutQty {
        get {
            return _OutQty;
        }
        set {
            _OutQty = value;
            OnPropertyChanged("OutQty");
        }
    }

    private decimal _LastQty;
    
    public decimal LastQty {
        get {
            return _LastQty;
        }
        set {
            _LastQty = value;
            OnPropertyChanged("LastQty");
        }
    }

    private decimal _MinUnitPrice;
    
    public decimal MinUnitPrice {
        get {
            return _MinUnitPrice;
        }
        set {
            _MinUnitPrice = value;
            OnPropertyChanged("MinUnitPrice");
        }
    }

    private decimal _MaxUnitPrice;
    
    public decimal MaxUnitPrice {
        get {
            return _MaxUnitPrice;
        }
        set {
            _MaxUnitPrice = value;
            OnPropertyChanged("MaxUnitPrice");
        }
    }

    private decimal _AvgUnitPrice;
    
    public decimal AvgUnitPrice {
        get {
            return _AvgUnitPrice;
        }
        set {
            _AvgUnitPrice = value;
            OnPropertyChanged("AvgUnitPrice");
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
