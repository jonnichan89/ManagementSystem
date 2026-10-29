using System;

namespace ManagementSystem.Data.Models;

[Table("ItemAdjustment")]
public partial class ItemAdjustment
{
    [Key]
    public int IAId { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime IADate { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime CreatedAt { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string CreatedBy { get; set; } = string.Empty;

    [Column(TypeName = "smalldatetime")]
    public DateTime ModifiedAt { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string ModifiedBy { get; set; } = string.Empty;

    public virtual ObservableCollection<ItemAdjustmentDetail> ItemAdjustmentDetails { get; set; } = new ObservableCollection<ItemAdjustmentDetail>();
}
