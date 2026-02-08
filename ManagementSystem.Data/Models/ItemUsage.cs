using System;

namespace ManagementSystem.Data.Models;

[Table("ItemUsage")]
public partial class ItemUsage
{
    [Key]
    public int ItemUsageId { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime ItemUsageDate { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime CreatedAt { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string CreatedBy { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime ModifiedAt { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string ModifiedBy { get; set; }

    public virtual ObservableCollection<ItemUsageDetail> ItemUsageDetails { get; set; } = new ObservableCollection<ItemUsageDetail>();
}
