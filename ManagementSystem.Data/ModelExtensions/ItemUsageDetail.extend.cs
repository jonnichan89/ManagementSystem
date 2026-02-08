using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace ManagementSystem.Data.Models;

public partial class ItemUsageDetail : INotifyPropertyChanged
{
    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    // Helper method to trigger the event
    public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    [NotMapped]
    public string ItemName { get; set; } = "";

    public void Clear()
    {
        this.ItemUsageId = 0;
        this.ItemUsageDetailId = 0;
        this.ItemCode = "";
        this.Qty = 0;
        this.CreatedAt = DateTime.Now;
        this.CreatedBy = "";
        this.ModifiedAt = DateTime.Now;
        this.ModifiedBy = "";

        this.ItemName = "";
    }

    public void CopyFrom(ItemUsageDetail source)
    {
        this.ItemUsageId = source.ItemUsageId;
        this.ItemUsageDetailId = source.ItemUsageDetailId;
        this.ItemCode = source.ItemCode;
        this.Qty = source.Qty;
        this.CreatedAt = source.CreatedAt;
        this.CreatedBy = source.CreatedBy;
        this.ModifiedAt = source.ModifiedAt;
        this.ModifiedBy = source.ModifiedBy;
        this.ItemName = source.ItemName;
    }
}
