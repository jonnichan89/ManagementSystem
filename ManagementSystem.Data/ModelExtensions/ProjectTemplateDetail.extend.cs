using System;

namespace ManagementSystem.Data.Models;

public partial class ProjectTemplateDetail
{
    public void SetDefault()
    {
        if (string.IsNullOrEmpty(ItemCode)) ItemCode = "";
        if (string.IsNullOrEmpty(ItemName)) ItemName = "";
        if (string.IsNullOrEmpty(Note)) Note = "";
        if (string.IsNullOrEmpty(CreatedBy)) CreatedBy = "";
        if (string.IsNullOrEmpty(ModifiedBy)) ModifiedBy = "";
    }
}
