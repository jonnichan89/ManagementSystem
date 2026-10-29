using System;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class ItemCalcDetail : INotifyPropertyChanged 
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

    private int _Id;
    
    public int Id {
        get {
            return _Id;
        }
        set {
            _Id = value;
            OnPropertyChanged("Id");
        }
    }

    private DateTime? _CalcDate;
    
    public DateTime? CalcDate {
        get {
            return _CalcDate;
        }
        set {
            _CalcDate = value;
            OnPropertyChanged("CalcDate");
        }
    }

    private string? _ItemCode = string.Empty;
    
    public string? ItemCode {
        get {
            return _ItemCode;
        }
        set {
            _ItemCode = value;
            OnPropertyChanged("ItemCode");
        }
    }

    private string? _CalcFromKey = string.Empty;
    
    public string? CalcFromKey {
        get {
            return _CalcFromKey;
        }
        set {
            _CalcFromKey = value;
            OnPropertyChanged("CalcFromKey");
        }
    }

    private decimal? _Qty;
    
    public decimal? Qty {
        get {
            return _Qty;
        }
        set {
            _Qty = value;
            OnPropertyChanged("Qty");
        }
    }

    private decimal? _Amount;
    
    public decimal? Amount {
        get {
            return _Amount;
        }
        set {
            _Amount = value;
            OnPropertyChanged("Amount");
        }
    }
}
