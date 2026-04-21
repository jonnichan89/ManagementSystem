using System;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class Finance : INotifyPropertyChanged 
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

    private string _FinanceCode = string.Empty;
    
    public string FinanceCode {
        get {
            return _FinanceCode;
        }
        set {
            _FinanceCode = value;
            OnPropertyChanged("FinanceCode");
        }
    }

    private DateTime _FinanceDate;
    
    public DateTime FinanceDate {
        get {
            return _FinanceDate;
        }
        set {
            _FinanceDate = value;
            OnPropertyChanged("FinanceDate");
        }
    }

    private string _FinanceType = string.Empty;
    
    public string FinanceType {
        get {
            return _FinanceType;
        }
        set {
            _FinanceType = value;
            OnPropertyChanged("FinanceType");
        }
    }

    private string _ReferenceType = string.Empty;
    
    public string ReferenceType {
        get {
            return _ReferenceType;
        }
        set {
            _ReferenceType = value;
            OnPropertyChanged("ReferenceType");
        }
    }

    private string _ReferenceCode = string.Empty;
    
    public string ReferenceCode {
        get {
            return _ReferenceCode;
        }
        set {
            _ReferenceCode = value;
            OnPropertyChanged("ReferenceCode");
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

    private string _SupplierCode = string.Empty;
    
    public string SupplierCode {
        get {
            return _SupplierCode;
        }
        set {
            _SupplierCode = value;
            OnPropertyChanged("SupplierCode");
        }
    }

    private string _SupplierName = string.Empty;
    
    public string SupplierName {
        get {
            return _SupplierName;
        }
        set {
            _SupplierName = value;
            OnPropertyChanged("SupplierName");
        }
    }

    private string _SupplierReferenceNo = string.Empty;
    
    public string SupplierReferenceNo {
        get {
            return _SupplierReferenceNo;
        }
        set {
            _SupplierReferenceNo = value;
            OnPropertyChanged("SupplierReferenceNo");
        }
    }

    private string _CurrencyId = string.Empty;
    
    public string CurrencyId {
        get {
            return _CurrencyId;
        }
        set {
            _CurrencyId = value;
            OnPropertyChanged("CurrencyId");
        }
    }

    private decimal _CurrencyRate;
    
    public decimal CurrencyRate {
        get {
            return _CurrencyRate;
        }
        set {
            _CurrencyRate = value;
            OnPropertyChanged("CurrencyRate");
        }
    }

    private decimal _PPNPercentage;
    
    public decimal PPNPercentage {
        get {
            return _PPNPercentage;
        }
        set {
            _PPNPercentage = value;
            OnPropertyChanged("PPNPercentage");
        }
    }

    private decimal _PPHPercentage;
    
    public decimal PPHPercentage {
        get {
            return _PPHPercentage;
        }
        set {
            _PPHPercentage = value;
            OnPropertyChanged("PPHPercentage");
        }
    }

    private decimal _DPP;
    
    public decimal DPP {
        get {
            return _DPP;
        }
        set {
            _DPP = value;
            OnPropertyChanged("DPP");
        }
    }

    private decimal _DPPDiscount;
    
    public decimal DPPDiscount {
        get {
            return _DPPDiscount;
        }
        set {
            _DPPDiscount = value;
            OnPropertyChanged("DPPDiscount");
        }
    }

    private decimal _PPN;
    
    public decimal PPN {
        get {
            return _PPN;
        }
        set {
            _PPN = value;
            OnPropertyChanged("PPN");
        }
    }

    private decimal _PPH;
    
    public decimal PPH {
        get {
            return _PPH;
        }
        set {
            _PPH = value;
            OnPropertyChanged("PPH");
        }
    }

    private decimal _TotalAmount;
    
    public decimal TotalAmount {
        get {
            return _TotalAmount;
        }
        set {
            _TotalAmount = value;
            OnPropertyChanged("TotalAmount");
        }
    }

    private bool _IsPaid;
    
    public bool IsPaid {
        get {
            return _IsPaid;
        }
        set {
            _IsPaid = value;
            OnPropertyChanged("IsPaid");
        }
    }

    private DateTime? _PaidAt;
    
    public DateTime? PaidAt {
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

    private bool _IsCancelled;
    
    public bool IsCancelled {
        get {
            return _IsCancelled;
        }
        set {
            _IsCancelled = value;
            OnPropertyChanged("IsCancelled");
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

    public virtual ObservableCollection<FinanceDetail> FinanceDetails { get; set; } = new ObservableCollection<FinanceDetail>();
}
