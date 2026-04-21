using System;

namespace ManagementSystem.Data.Models;

public partial class PSOrderDetail
{
    public void SetDefault()
    {
        if (string.IsNullOrEmpty(ItemCode)) ItemCode = "";
        if (string.IsNullOrEmpty(ItemName)) ItemName = "";
        if (string.IsNullOrEmpty(Unit)) Unit = "";
    }
}
