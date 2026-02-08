using System;

namespace ManagementSystem.Data.Models;

[PrimaryKey("ItemUsageId", "ItemUsageDetailId")]
[Table("ItemUsageDetail")]
public partial class ItemUsageDetail
{
    [Key]
    public int ItemUsageId { get; set; }

    [Key]
    public int ItemUsageDetailId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ItemCode { get; set; }

    [Column(TypeName = "money")]
    public decimal Qty { get; set; }

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

    [ForeignKey("ItemUsageId")]
    [InverseProperty("ItemUsageDetails")]
    public virtual ItemUsage ItemUsage { get; set; }
}
