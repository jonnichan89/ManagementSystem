using System;

namespace ManagementSystem.Data.Models;

[PrimaryKey("FinanceId", "FinanceDetailId")]
[Table("FinanceDetail")]
public partial class FinanceDetail
{
    [Key]
    public int FinanceId { get; set; }

    [Key]
    public int FinanceDetailId { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime PaidAt { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string PaidCurrencyId { get; set; } = string.Empty;

    [Column(TypeName = "money")]
    public decimal PaidCurrencyRate { get; set; }

    [Column(TypeName = "money")]
    public decimal PaidAmount { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string PaidNote { get; set; } = string.Empty;

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

    [ForeignKey("FinanceId")]
    [InverseProperty("FinanceDetails")]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual Finance? Finance { get; set; } = null;
}
