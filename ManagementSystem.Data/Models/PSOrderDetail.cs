
namespace ManagementSystem.Data.Models;

[PrimaryKey("PSId", "PSDId")]
[Table("PSOrderDetail")]
public partial class PSOrderDetail
{
    [Key]
    public int PSId { get; set; }

    [Key]
    public int PSDId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ItemCode { get; set; } = string.Empty;

    [StringLength(500)]
    [Unicode(false)]
    public string ItemName { get; set; } = string.Empty;

    [Column(TypeName = "money")]
    public decimal Qty { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Unit { get; set; } = string.Empty;

    [Column(TypeName = "money")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "money")]
    public decimal UnitPriceDiscount { get; set; }

    [Column(TypeName = "money")]
    public decimal DPP { get; set; }

    [Column(TypeName = "money")]
    public decimal DPPDiscount { get; set; }

    [Column(TypeName = "money")]
    public decimal TotalAmount { get; set; }

    [ForeignKey("PSId")]
    [InverseProperty("PSOrderDetails")]
    public virtual PSOrder? PSOrder { get; set; } = null;
}
