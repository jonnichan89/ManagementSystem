using System;

namespace ManagementSystem.Data.Models;

[Table("MstItem")]
public partial class MstItem
{
    [Key]
    public int ItemId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ItemCode { get; set; } = string.Empty;

    [StringLength(500)]
    [Unicode(false)]
    public string ItemName { get; set; } = string.Empty;

    [StringLength(1000)]
    [Unicode(false)]
    public string ItemDescription { get; set; } = string.Empty;

    [StringLength(20)]
    [Unicode(false)]
    public string ItemUnit { get; set; } = string.Empty;

    [StringLength(500)]
    [Unicode(false)]
    public string ImagePath { get; set; } = string.Empty;

    public bool IsStockControl { get; set; }

    public bool IsBlacklist { get; set; }

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

    public int MinimumStock { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string SupplierCode { get; set; } = string.Empty;
}
