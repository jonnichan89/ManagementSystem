using Microsoft.AspNetCore.Http;
using System;

namespace ManagementSystem.Data.Models;

public partial class ProjectTemplate
{
    public void SetDefault()
    {
        if (string.IsNullOrEmpty(PTName)) PTName = "";
        if (string.IsNullOrEmpty(Note)) Note = "";
        if (string.IsNullOrEmpty(ImagePath)) ImagePath = "";
        if (string.IsNullOrEmpty(CreatedBy)) CreatedBy = "";
        if (string.IsNullOrEmpty(ModifiedBy)) ModifiedBy = "";

        foreach (ProjectTemplateDetail item in ProjectTemplateDetails)
        {
            item.SetDefault();
        }
    }

    [NotMapped]
    public bool HasImageUpload { get; set; }
}
