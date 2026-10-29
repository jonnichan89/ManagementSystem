
namespace ManagementSystem.Data.Models;

[PrimaryKey("PSId", "PSTId")]
[Table("PSOrderPT")]
public partial class PSOrderPT
{
    [Key]
    public int PSId { get; set; }

    [Key]
    public int PSTId { get; set; }

    public int PTId { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string PTName { get; set; } = string.Empty;

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
    [InverseProperty("PSOrderPTs")]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual PSOrder? PSOrder { get; set; } = null;
}
