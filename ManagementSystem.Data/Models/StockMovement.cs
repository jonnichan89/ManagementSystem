using System;

namespace ManagementSystem.Data.Models;

[Table("StockMovement")]
public partial class StockMovement
{
    [Key]
    public int SMId { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime SMDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ItemCode { get; set; } = string.Empty;

    public int POId { get; set; }

    [Column(TypeName = "money")]
    public decimal POQty { get; set; }

    [Column(TypeName = "money")]
    public decimal POUsedQty { get; set; }

    public int SOId { get; set; }

    [Column(TypeName = "money")]
    public decimal SOQty { get; set; }

    [Column(TypeName = "money")]
    public decimal SOUsedQty { get; set; }

    [Column(TypeName = "money")]
    public decimal SOUseSMId { get; set; }

    [Column(TypeName = "money")]
    public decimal SOUseSMPOQty { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime LastModifiedAt { get; set; }
}
