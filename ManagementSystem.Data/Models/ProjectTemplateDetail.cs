using System;

namespace ManagementSystem.Data.Models;

[PrimaryKey("PTId", "PTDId")]
[Table("ProjectTemplateDetail")]
public partial class ProjectTemplateDetail
{
    [Key]
    public int PTId { get; set; }

    [Key]
    public int PTDId { get; set; }

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

    [StringLength(8000)]
    [Unicode(false)]
    public string Note { get; set; } = string.Empty;

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

    [ForeignKey("PTId")]
    [InverseProperty("ProjectTemplateDetails")]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ProjectTemplate? ProjectTemplate { get; set; } = null;
}
