
using System.Diagnostics.Eventing.Reader;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class Finance
{
    public Action DelegadeReferenceTypeChanged; // (MstSupplier? selectedSupplier);
    public void AddFinanceDetail()
    {
        FinanceDetails.Add(new FinanceDetail()
        {
            Finance = this,
            PaidCurrencyId = "IDR",
            PaidAt = DateTime.Today,
            PaidCurrencyRate = 1,
            CreatedAt = DateTime.Now,
            CreatedBy = "",
            ModifiedAt = DateTime.Now,
            ModifiedBy = ""
        });
    }

    #region ExtendedProperty

    public CurrencyIdEnum CurrencyIdEnumValue
    {
        get
        {
            switch (CurrencyId)
            {
                case "IDR":
                    return CurrencyIdEnum.IDR;
                case "CNY":
                    return CurrencyIdEnum.CNY;
                case "USD":
                    return CurrencyIdEnum.USD;
                default:
                    return CurrencyIdEnum.IDR;
            }
        }
        set
        {
            CurrencyId = value.ToString();
            OnPropertyChanged(nameof(CurrencyIdEnumValue));
        }
    }

    public int Rounding
    {
        get
        {
            switch (_CurrencyId)
            {
                case "IDR":
                    return 0;
                case "CNY":
                    return 2;
                case "USD":
                    return 2;
                default:
                    return 0;
            }
        }
    }

    public bool N0Visibility
    {
        get
        {
            switch (_CurrencyId)
            {
                case "IDR":
                    return true;
                case "CNY":
                    return false;
                case "USD":
                    return false;
                default:
                    return true;
            }
        }
    }

    public bool N2Visibility
    {
        get
        {
            switch (_CurrencyId)
            {
                case "IDR":
                    return false;
                case "CNY":
                    return true;
                case "USD":
                    return true;
                default:
                    return false;
            }
        }
    }

    public decimal DPPIDR
    {
        get
        {
            return _CurrencyRate * _DPP;
        }
    }

    public decimal DPPIDRDiscount
    {
        get
        {
            return _CurrencyRate * _DPPDiscount;
        }
    }

    public decimal TotalAmountIDR
    {
        get
        {
            return _CurrencyRate * _TotalAmount;
        }
    }

    public bool RTMenuPurchase
    {
        get
        {
            return _ReferenceType.Contains("RTPO");
        }
        set
        {
            if (!RTMenuPurchase)
            {
                _ReferenceType = _ReferenceType.
                    Replace("RTPO", "").
                    Replace("RTSO", "").
                    Replace("RTO", "") + "RTPO";

                RaiseRTPropertyChanged();
            }
        }
    }

    public bool RTMenuSales
    {
        get
        {
            return _ReferenceType.Contains("RTSO");
        }
        set
        {
            if (!RTMenuSales)
            {
                _ReferenceType = _ReferenceType.
                    Replace("RTPO", "").
                    Replace("RTSO", "").
                    Replace("RTO", "") + "RTSO";

                RaiseRTPropertyChanged();
            }
        }
    }

    public bool RTOther
    {
        get
        {
            return _ReferenceType.Contains("RTO");
        }
        set
        {
            if (!RTOther)
            {
                _ReferenceType = _ReferenceType.
                    Replace("RTPO", "").
                    Replace("RTSO", "").
                    Replace("RTO", "") + "RTO";

                RaiseRTPropertyChanged();
            }
        }
    }

    public bool IsRTPS { get => RTMenuPurchase || RTMenuSales; }
    public bool IsRTOther { get => RTOther; }

    public bool FTAPDebitNote
    {
        get
        {
            return _FinanceType.Contains("TAPDN");
        }
        set
        {
            if (!FTAPDebitNote)
            {
                _FinanceType = _FinanceType.
                    Replace("TAPDN", "").
                    Replace("TAPCN", "").
                    Replace("TARDN", "").
                    Replace("TARCN", "") + "TAPDN";

                RaiseFTPropertyChanged();
            }
        }
    }
    public bool FTAPCreditNote
    {
        get
        {
            return _FinanceType.Contains("TAPCN");
        }
        set
        {
            if (!FTAPCreditNote)
            {
                _FinanceType = _FinanceType.
                    Replace("TAPDN", "").
                    Replace("TAPCN", "").
                    Replace("TARDN", "").
                    Replace("TARCN", "") + "TAPCN";

                RaiseFTPropertyChanged();
            }
        }
    }
    public bool FTARDebitNote
    {
        get
        {
            return _FinanceType.Contains("TARDN");
        }
        set
        {
            if (!FTARDebitNote)
            {
                _FinanceType = _FinanceType.
                    Replace("TAPDN", "").
                    Replace("TAPCN", "").
                    Replace("TARDN", "").
                    Replace("TARCN", "") + "TARDN";

                RaiseFTPropertyChanged();
            }
        }
    }
    public bool FTARCreditNote
    {
        get
        {
            return _FinanceType.Contains("TARCN");
        }
        set
        {
            if (!FTARCreditNote)
            {
                _FinanceType = _FinanceType.
                    Replace("TAPDN", "").
                    Replace("TAPCN", "").
                    Replace("TARDN", "").
                    Replace("TARCN", "") + "TARCN";

                RaiseFTPropertyChanged();
            }
        }
    }

    private void RaiseRTPropertyChanged()
    {
        OnPropertyChanged("RTMenuSales");
        OnPropertyChanged("RTMenuPurchase");
        OnPropertyChanged("RTOther");
        OnPropertyChanged("IsRTPS");
        OnPropertyChanged("IsRTOther");

        if (DelegadeReferenceTypeChanged != null)
            DelegadeReferenceTypeChanged();
    }

    private void RaiseFTPropertyChanged()
    {
        OnPropertyChanged("FTAPDebitNote");
        OnPropertyChanged("FTAPCreditNote");
        OnPropertyChanged("FTARDebitNote");
        OnPropertyChanged("FTARCreditNote");
    }

    #endregion


    private bool _doNotRecalculate = false;
    partial void AfterPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case "CurrencyId":
                OnPropertyChanged("N0Visibility");
                OnPropertyChanged("N2Visibility");
                if (CurrencyId == "IDR")
                {
                    CurrencyRate = 1;
                }
                break;

            case "DPP":
            case "DPPDiscount":
                RecalculateTotalAmount();
                break;
        }
    }

    public void RecalculateTotalAmount()
    {
        if (_doNotRecalculate) return;

        TotalAmount = DPP - DPPDiscount;
        OnPropertyChanged("DPPIDR");
        OnPropertyChanged("DPPIDRDiscount");
        OnPropertyChanged("TotalAmountIDR");
    }
}
