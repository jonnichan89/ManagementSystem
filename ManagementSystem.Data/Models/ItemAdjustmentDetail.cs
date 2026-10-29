using System;

namespace ManagementSystem.Data.Models;

[PrimaryKey("IAId", "IADId")]
[Table("ItemAdjustmentDetail")]
public partial class ItemAdjustmentDetail
{
    [Key]
    public int IAId { get; set; }

    [Key]
    public int IADId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ItemCode { get; set; } = string.Empty;

    [Column(TypeName = "money")]
    public decimal Qty { get; set; }

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

    [ForeignKey("IAId")]
    [InverseProperty("ItemAdjustmentDetails")]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ItemAdjustment? ItemAdjustment { get; set; } = null;
}
