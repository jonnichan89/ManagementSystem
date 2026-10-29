
using System.Diagnostics.Eventing.Reader;

namespace ManagementSystem.WPF.UI.Data.DBModels;

public partial class ProjectTemplate
{
    #region Search
    public string SearchKey { get { return PTName; } }

    public bool PutSearchToTop
    {
        set
        {
            Askterisk = (value ? "*" : "");
        }
    }

    public string Askterisk { get; private set; } = "";

    #endregion

    public bool IsMainProject
    {
        get {
            return !_ParentPTId.HasValue;
        }
    }

    public bool IsFirstProject
    {
        get {
            return PTNo == 1;
        }
    }

    public string DisplayPTName
    {
        get
        {
            if (IsMainProject)
                return ProjectTemplates.FirstOrDefault()?.PTName ?? "";
            else
                return "";
        }
    }

    public void AddProjectTemplate()
    {
        byte PTNo = (byte)1;

        if (ProjectTemplates.Count > 0)
            PTNo = (byte)(ProjectTemplates.Max(p => p.PTNo) + 1);

        ProjectTemplates.Add(new ProjectTemplate()
        {
            ParentPTId = PTId,
            ParentProjectTemplate = this,
            PTNo = PTNo,
            PTName = "Project",
            Note = "",
            ImagePath = "",
            ImagePathToUpload = "",
            CreatedAt = DateTime.Now,
            CreatedBy = "",
            ModifiedAt = DateTime.Now,
            ModifiedBy = ""
        });
    }

    public void AddProjectTemplateDetail()
    {
        byte PTDNo = (byte)1;

        if (ProjectTemplateDetails.Count > 0)
            PTDNo = (byte)(ProjectTemplateDetails.Max(p => p.PTDId) + 1);

        ProjectTemplateDetails.Add(new ProjectTemplateDetail()
        {
            ProjectTemplate = this,
            PTId = this.PTId,
            PTDId = PTDNo,
            ItemCode = "",
            ItemName = "",
            Qty = 0,
            Note = "",
        });
    }

    private string _ImagePathToUpload;

    public string ImagePathToUpload
    {
        get
        {
            return _ImagePathToUpload;
        }
        set
        {
            _ImagePathToUpload = value;
            OnPropertyChanged("ImagePathToUpload");
        }
    }

    private bool _doNotRecalculate = false;
    partial void AfterPropertyChanged(string propertyName)
    {
        //switch (propertyName)
        //{
        //    case "PaidCurrencyId":
        //        if (PaidCurrencyId == "IDR")
        //        {
        //            PaidCurrencyRate = 1;
        //        }
        //        OnPropertyChanged("PaidAmountIDR");
        //        break;
        //}
    }
}
