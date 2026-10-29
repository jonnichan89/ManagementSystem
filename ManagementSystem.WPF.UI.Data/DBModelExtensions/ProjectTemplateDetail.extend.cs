
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class ProjectTemplateDetail
{
    public ProjectTemplateDetail()
    {
        Clear();
    }

    public void Clear()
    {
        this.PTId = 0;
        this.PTDId = 0;
        this.ItemCode = "";
        this.ItemName = "";
        this.Qty = 0;
        this.Note = "";
    }

    public void CopyFrom(ProjectTemplateDetail source)
    {
        this.PTId = source.PTId;
        this.PTDId = source.PTDId;
        this.ItemCode = source.ItemCode;
        this.ItemName = source.ItemName;
        this.Qty = source.Qty;
    }

    private bool _doNotRecalculate = false;

    partial void AfterPropertyChanged(string propertyName)
    {
        //switch (propertyName) {
        //    case "Qty":
        //    case "UnitPrice":
        //    case "UnitPriceDiscount":
        //        RecalculateDPP();
        //        break;
        //    case "DPP":
        //    case "DPPDiscount":
        //        RecalculateTotalAmount();
        //        break;
        //}
    }
}
