using System;

namespace ManagementSystem.Data.Models;

[PrimaryKey("SSDate", "ItemCode")]
[Table("StockSummary")]
public partial class StockSummary
{
    [Key]
    [Column(TypeName = "smalldatetime")]
    public DateTime SSDate { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string ItemCode { get; set; }

    [Column(TypeName = "money")]
    public decimal BeginQty { get; set; }

    [Column(TypeName = "money")]
    public decimal InQty { get; set; }

    [Column(TypeName = "money")]
    public decimal OutQty { get; set; }

    [Column(TypeName = "money")]
    public decimal LastQty { get; set; }

    [Column(TypeName = "money")]
    public decimal MinUnitPrice { get; set; }

    [Column(TypeName = "money")]
    public decimal MaxUnitPrice { get; set; }

    [Column(TypeName = "money")]
    public decimal AvgUnitPrice { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime LastModifiedAt { get; set; }
}
