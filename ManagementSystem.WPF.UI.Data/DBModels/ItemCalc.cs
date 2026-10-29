using System;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class ItemCalc : INotifyPropertyChanged 
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

    private decimal? _POQty;
    
    public decimal? POQty {
        get {
            return _POQty;
        }
        set {
            _POQty = value;
            OnPropertyChanged("POQty");
        }
    }

    private decimal? _SOQty;
    
    public decimal? SOQty {
        get {
            return _SOQty;
        }
        set {
            _SOQty = value;
            OnPropertyChanged("SOQty");
        }
    }

    private decimal? _IUQty;
    
    public decimal? IUQty {
        get {
            return _IUQty;
        }
        set {
            _IUQty = value;
            OnPropertyChanged("IUQty");
        }
    }

    private decimal? _IAQty;
    
    public decimal? IAQty {
        get {
            return _IAQty;
        }
        set {
            _IAQty = value;
            OnPropertyChanged("IAQty");
        }
    }

    private decimal? _SPQty;
    
    public decimal? SPQty {
        get {
            return _SPQty;
        }
        set {
            _SPQty = value;
            OnPropertyChanged("SPQty");
        }
    }

    private decimal? _InQty;
    
    public decimal? InQty {
        get {
            return _InQty;
        }
        set {
            _InQty = value;
            OnPropertyChanged("InQty");
        }
    }

    private decimal? _InAmount;
    
    public decimal? InAmount {
        get {
            return _InAmount;
        }
        set {
            _InAmount = value;
            OnPropertyChanged("InAmount");
        }
    }

    private decimal? _OutQty;
    
    public decimal? OutQty {
        get {
            return _OutQty;
        }
        set {
            _OutQty = value;
            OnPropertyChanged("OutQty");
        }
    }

    private decimal? _OutAmount;
    
    public decimal? OutAmount {
        get {
            return _OutAmount;
        }
        set {
            _OutAmount = value;
            OnPropertyChanged("OutAmount");
        }
    }

    private decimal? _BeforeQty;
    
    public decimal? BeforeQty {
        get {
            return _BeforeQty;
        }
        set {
            _BeforeQty = value;
            OnPropertyChanged("BeforeQty");
        }
    }

    private decimal? _BeforeUP;
    
    public decimal? BeforeUP {
        get {
            return _BeforeUP;
        }
        set {
            _BeforeUP = value;
            OnPropertyChanged("BeforeUP");
        }
    }

    private decimal? _BeforeAmount;
    
    public decimal? BeforeAmount {
        get {
            return _BeforeAmount;
        }
        set {
            _BeforeAmount = value;
            OnPropertyChanged("BeforeAmount");
        }
    }

    private decimal? _AfterQty;
    
    public decimal? AfterQty {
        get {
            return _AfterQty;
        }
        set {
            _AfterQty = value;
            OnPropertyChanged("AfterQty");
        }
    }

    private decimal? _AfterUP;
    
    public decimal? AfterUP {
        get {
            return _AfterUP;
        }
        set {
            _AfterUP = value;
            OnPropertyChanged("AfterUP");
        }
    }

    private decimal? _AfterAmount;
    
    public decimal? AfterAmount {
        get {
            return _AfterAmount;
        }
        set {
            _AfterAmount = value;
            OnPropertyChanged("AfterAmount");
        }
    }

    private DateTime? _CalcAt;
    
    public DateTime? CalcAt {
        get {
            return _CalcAt;
        }
        set {
            _CalcAt = value;
            OnPropertyChanged("CalcAt");
        }
    }
}
