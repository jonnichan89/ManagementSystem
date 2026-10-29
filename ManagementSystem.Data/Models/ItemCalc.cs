using System;

namespace ManagementSystem.Data.Models;

[Table("ItemCalc")]
[Index("CalcDate", "ItemCode", Name = "IX_ItemCalc_1")]
public partial class ItemCalc
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime? CalcDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ItemCode { get; set; } = string.Empty;

    [Column(TypeName = "money")]
    public decimal? POQty { get; set; }

    [Column(TypeName = "money")]
    public decimal? SOQty { get; set; }

    [Column(TypeName = "money")]
    public decimal? IUQty { get; set; }

    [Column(TypeName = "money")]
    public decimal? IAQty { get; set; }

    [Column(TypeName = "money")]
    public decimal? SPQty { get; set; }

    [Column(TypeName = "money")]
    public decimal? InQty { get; set; }

    [Column(TypeName = "money")]
    public decimal? InAmount { get; set; }

    [Column(TypeName = "money")]
    public decimal? OutQty { get; set; }

    [Column(TypeName = "money")]
    public decimal? OutAmount { get; set; }

    [Column(TypeName = "money")]
    public decimal? BeforeQty { get; set; }

    [Column(TypeName = "money")]
    public decimal? BeforeUP { get; set; }

    [Column(TypeName = "money")]
    public decimal? BeforeAmount { get; set; }

    [Column(TypeName = "money")]
    public decimal? AfterQty { get; set; }

    [Column(TypeName = "money")]
    public decimal? AfterUP { get; set; }

    [Column(TypeName = "money")]
    public decimal? AfterAmount { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime? CalcAt { get; set; }
}
