using System;

namespace ManagementSystem.Data.Models;

[Table("MstSupplier")]
public partial class MstSupplier
{
    [Key]
    public int SupplierId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string SupplierCode { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string SupplierName { get; set; }

    public bool IsBlacklist { get; set; }

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
}
