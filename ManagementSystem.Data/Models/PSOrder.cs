using System;

namespace ManagementSystem.Data.Models;

[Table("PSOrder")]
public partial class PSOrder
{
    [Key]
    public int PSId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string PSCode { get; set; } = string.Empty;

    [Column(TypeName = "smalldatetime")]
    public DateTime PSDate { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string PSType { get; set; } = string.Empty;

    [StringLength(50)]
    [Unicode(false)]
    public string SupplierCode { get; set; } = string.Empty;

    [StringLength(50)]
    [Unicode(false)]
    public string SupplierName { get; set; } = string.Empty;

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

    [Column(TypeName = "smalldatetime")]
    public DateTime? DispatchDate { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime? IncomingDate { get; set; }

    public virtual ObservableCollection<PSOrderDetail> PSOrderDetails { get; set; } = new ObservableCollection<PSOrderDetail>();

    public virtual ObservableCollection<PSOrderPT> PSOrderPTs { get; set; } = new ObservableCollection<PSOrderPT>();
}
