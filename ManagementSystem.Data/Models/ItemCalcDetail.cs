using System;

namespace ManagementSystem.Data.Models;

[Table("ItemCalcDetail")]
[Index("CalcDate", "ItemCode", Name = "IX_ItemCalcDetail_1")]
public partial class ItemCalcDetail
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime? CalcDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ItemCode { get; set; } = string.Empty;

    [StringLength(50)]
    [Unicode(false)]
    public string? CalcFromKey { get; set; } = string.Empty;

    [Column(TypeName = "money")]
    public decimal? Qty { get; set; }

    [Column(TypeName = "money")]
    public decimal? Amount { get; set; }
}
