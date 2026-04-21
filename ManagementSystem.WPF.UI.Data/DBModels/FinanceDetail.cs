using System;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class FinanceDetail : INotifyPropertyChanged 
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

    private int _FinanceId;
    
    public int FinanceId {
        get {
            return _FinanceId;
        }
        set {
            _FinanceId = value;
            OnPropertyChanged("FinanceId");
        }
    }

    private int _FinanceDetailId;
    
    public int FinanceDetailId {
        get {
            return _FinanceDetailId;
        }
        set {
            _FinanceDetailId = value;
            OnPropertyChanged("FinanceDetailId");
        }
    }

    private DateTime _PaidAt;
    
    public DateTime PaidAt {
        get {
            return _PaidAt;
        }
        set {
            _PaidAt = value;
            OnPropertyChanged("PaidAt");
        }
    }

    private string _PaidCurrencyId = string.Empty;
    
    public string PaidCurrencyId {
        get {
            return _PaidCurrencyId;
        }
        set {
            _PaidCurrencyId = value;
            OnPropertyChanged("PaidCurrencyId");
        }
    }

    private decimal _PaidCurrencyRate;
    
    public decimal PaidCurrencyRate {
        get {
            return _PaidCurrencyRate;
        }
        set {
            _PaidCurrencyRate = value;
            OnPropertyChanged("PaidCurrencyRate");
        }
    }

    private decimal _PaidAmount;
    
    public decimal PaidAmount {
        get {
            return _PaidAmount;
        }
        set {
            _PaidAmount = value;
            OnPropertyChanged("PaidAmount");
        }
    }

    private string _PaidNote = string.Empty;
    
    public string PaidNote {
        get {
            return _PaidNote;
        }
        set {
            _PaidNote = value;
            OnPropertyChanged("PaidNote");
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

    private Finance _Finance = null!;
    
    public virtual Finance Finance {
        get {
            return _Finance;
        }
        set {
            _Finance = value;
            OnPropertyChanged("Finance");
        }
    }
}
