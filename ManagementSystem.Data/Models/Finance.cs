using System;

namespace ManagementSystem.Data.Models;

[Table("Finance")]
public partial class Finance
{
    [Key]
    public int FinanceId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string FinanceCode { get; set; } = string.Empty;

    [Column(TypeName = "smalldatetime")]
    public DateTime FinanceDate { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string FinanceType { get; set; } = string.Empty;

    [StringLength(10)]
    [Unicode(false)]
    public string ReferenceType { get; set; } = string.Empty;

    [StringLength(50)]
    [Unicode(false)]
    public string ReferenceCode { get; set; } = string.Empty;

    [StringLength(50)]
    [Unicode(false)]
    public string AccountCode { get; set; } = string.Empty;

    [StringLength(300)]
    [Unicode(false)]
    public string AccountName { get; set; } = string.Empty;

    [StringLength(50)]
    [Unicode(false)]
    public string SupplierCode { get; set; } = string.Empty;

    [StringLength(50)]
    [Unicode(false)]
    public string SupplierName { get; set; } = string.Empty;

    [StringLength(50)]
    [Unicode(false)]
    public string SupplierReferenceNo { get; set; } = string.Empty;

    [StringLength(20)]
    [Unicode(false)]
    public string CurrencyId { get; set; } = string.Empty;

    [Column(TypeName = "money")]
    public decimal CurrencyRate { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal PPNPercentage { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal PPHPercentage { get; set; }

    [Column(TypeName = "money")]
    public decimal DPP { get; set; }

    [Column(TypeName = "money")]
    public decimal DPPDiscount { get; set; }

    [Column(TypeName = "money")]
    public decimal PPN { get; set; }

    [Column(TypeName = "money")]
    public decimal PPH { get; set; }

    [Column(TypeName = "money")]
    public decimal TotalAmount { get; set; }

    public bool IsPaid { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime? PaidAt { get; set; }

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

    public bool IsCancelled { get; set; }

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

    public virtual ObservableCollection<FinanceDetail> FinanceDetails { get; set; } = new ObservableCollection<FinanceDetail>();
}
