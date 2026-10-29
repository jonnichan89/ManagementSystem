
namespace ManagementSystem.Data.Models;

[PrimaryKey("POId", "PODId")]
[Table("PurchaseOrderDetail")]
public partial class PurchaseOrderDetail
{
    [Key]
    public int POId { get; set; }

    [Key]
    public int PODId { get; set; }

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

    [ForeignKey("POId")]
    [InverseProperty("PurchaseOrderDetails")]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual PurchaseOrder? PurchaseOrder { get; set; } = null;
}
