
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class PSOrderPT
{
    public PSOrderPT() {
        Clear();
    }

    public void Clear()
    {
        this.PSId = 0;
        this.PSTId = 0;
        this.PTId = 0;
        this.PTName = "";
        this.Qty = 0;
        this.Unit = "";
        this.UnitPrice = 0;
        this.UnitPriceDiscount = 0;
        this.DPP = 0;
        this.DPPDiscount = 0;
        this.TotalAmount = 0;
    }

    public void CopyFrom(PSOrderPT source)
    {
        this.PSId = source.PSId;
        this.PSTId = source.PSTId;
        this.PTId = source.PTId;
        this.PTName = source.PTName;
        this.Qty = source.Qty;
        this.Unit = source.Unit;
        this.UnitPrice = source.UnitPrice;
        this.UnitPriceDiscount = source.UnitPriceDiscount;
        this.DPP = source.DPP;
        this.DPPDiscount = source.DPPDiscount;
        this.TotalAmount = source.TotalAmount;
    }

    private bool _doNotRecalculate = false;

    partial void AfterPropertyChanged(string propertyName)
    {
        switch (propertyName) {
            case "Qty":
            case "UnitPrice":
            case "UnitPriceDiscount":
                RecalculateDPP();
                break;
            case "DPP":
            case "DPPDiscount":
                RecalculateTotalAmount();
                break;
        }
    }

    internal void RecalculateDPP()
    {
        if (PSOrder == null) return;

        DPP = Math.Round( Qty * (UnitPrice - UnitPriceDiscount), PSOrder.Rounding);
    }

    private void RecalculateTotalAmount()
    {
        if (_doNotRecalculate) return;

        TotalAmount = DPP - DPPDiscount;
        if (PSOrder != null)
        {
            PSOrder.ItemChanged();
        }
    }
}
