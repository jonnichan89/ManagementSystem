
using System.Diagnostics.Eventing.Reader;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class PurchaseOrder
{
    public void AddPurchaseOrderDetail()
    {
        PurchaseOrderDetails.Add(new PurchaseOrderDetail()
        {
            PurchaseOrder = this
        });
    }

    #region ExtendedProperty

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
        get {
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
                foreach (PurchaseOrderDetail item in PurchaseOrderDetails)
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
        foreach (PurchaseOrderDetail item in PurchaseOrderDetails)
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
