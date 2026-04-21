
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class FinanceDetail
{
    public FinanceDetail() {
        Clear();
    }

    public void Clear()
    {
        this.FinanceId = 0;
        this.FinanceDetailId = 0;
    }

    public void CopyFrom(FinanceDetail source)
    {
        this.FinanceId = source.FinanceId;
        this.FinanceDetailId = source.FinanceDetailId;
    }

    #region ExtendedProperty

    public int PaidRounding
    {
        get
        {
            switch (_PaidCurrencyId)
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

    public bool PaidN0Visibility
    {
        get
        {
            switch (_PaidCurrencyId)
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

    public bool PaidN2Visibility
    {
        get
        {
            switch (_PaidCurrencyId)
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

    public decimal PaidAmountIDR
    {
        get
        {
            return _PaidCurrencyRate * _PaidAmount;
        }
    }

    public CurrencyIdEnum PaidCurrencyIdEnumValue
    {
        get
        {
            switch (PaidCurrencyId)
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
            PaidCurrencyId = value.ToString();
            OnPropertyChanged(nameof(PaidCurrencyIdEnumValue));
        }
    }

    #endregion

    private bool _doNotRecalculate = false;

    partial void AfterPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case "PaidCurrencyId":
                OnPropertyChanged("PaidN0Visibility");
                OnPropertyChanged("PaidN0Visibility");
                if (PaidCurrencyId == "IDR")
                {
                    PaidCurrencyRate = 1;
                }
                OnPropertyChanged("PaidAmountIDR");
                break;
        }
    }
}
