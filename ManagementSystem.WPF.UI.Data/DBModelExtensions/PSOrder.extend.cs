
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class PSOrder
{
    public void AddPSOrderDetail()
    {
        int lastPSDId = PSOrderDetails.LastOrDefault()?.PSDId ?? 0;

        PSOrderDetails.Add(new PSOrderDetail()
        {
            PSOrder = this,
            PSDId = lastPSDId + 1
        });
    }

    public void RemovePSOrderDetail(PSOrderDetail psd)
    {
        PSOrderDetails.Remove(psd);

        int psdId = 0;
        foreach (PSOrderDetail psd1 in PSOrderDetails.OrderBy(p => p.PSDId))
        {
            psdId++;
            psd1.PSDId = psdId;
        }
    }

    public void AddPSOrderPT()
    {
        int lastPSTId = PSOrderPTs.LastOrDefault()?.PSTId ?? 0;

        PSOrderPTs.Add(new PSOrderPT()
        {
            PSOrder = this,
            PSTId = lastPSTId + 1
        });
    }

    public void RemovePSOrderPT(PSOrderPT pst)
    {
        PSOrderPTs.Remove(pst);

        int pstId = 0;
        foreach (PSOrderPT pst1 in PSOrderPTs.OrderBy(p => p.PSTId))
        {
            pstId++;
            pst1.PSTId = pstId;
        }
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

    public decimal PaidAmountIDR
    {
        get
        {
            return _PaidCurrencyRate * _PaidAmount;
        }
    }

    #endregion


    private bool _doNotRecalculate = false;
    partial void AfterPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case "PaidCurrencyId":
                if (PaidCurrencyId == "IDR")
                {
                    PaidCurrencyRate = 1;
                }
                OnPropertyChanged("PaidAmountIDR");
                break;

            case "CurrencyId":
                OnPropertyChanged("N0Visibility");
                OnPropertyChanged("N2Visibility");
                if (CurrencyId == "IDR")
                {
                    CurrencyRate = 1;
                }

                _doNotRecalculate = true;
                foreach (PSOrderDetail item in PSOrderDetails)
                {
                    item.RecalculateDPP();
                }
                _doNotRecalculate = false;
                ItemChanged();
                break;

            case "DPP":
            case "DPPDiscount":
                RecalculateTotalAmount();
                break;

            case "PaidAmount":
                OnPropertyChanged("PaidAmountIDR");
                break;
        }
    }

    public void ItemChanged()
    {
        if (_doNotRecalculate) return;

        _doNotRecalculate = true;

        decimal dpp = 0;
        foreach (PSOrderDetail item in PSOrderDetails)
        {
            dpp += item.DPP - item.DPPDiscount;
        }
        DPP = dpp;

        _doNotRecalculate = false;

        RecalculateTotalAmount();
    }

    public void RecalculateTotalAmount()
    {
        if (_doNotRecalculate) return;

        TotalAmount = DPP - DPPDiscount;
    }
}
