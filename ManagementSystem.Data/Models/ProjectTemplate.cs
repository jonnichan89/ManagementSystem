using System;

namespace ManagementSystem.Data.Models;

[Table("ProjectTemplate")]
public partial class ProjectTemplate
{
    [Key]
    public int PTId { get; set; }

    public byte? PTNo { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string PTName { get; set; } = string.Empty;

    [StringLength(8000)]
    [Unicode(false)]
    public string Note { get; set; } = string.Empty;

    [StringLength(500)]
    [Unicode(false)]
    public string ImagePath { get; set; } = string.Empty;

    public int ImageFileSize { get; set; }

    public int? ParentPTId { get; set; }

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

    public virtual ObservableCollection<ProjectTemplate> ProjectTemplates { get; set; } = new ObservableCollection<ProjectTemplate>();

    [ForeignKey("ParentPTId")]
    [InverseProperty("ProjectTemplates")]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ProjectTemplate? ParentProjectTemplate { get; set; } = null;

    public virtual ObservableCollection<ProjectTemplateDetail> ProjectTemplateDetails { get; set; } = new ObservableCollection<ProjectTemplateDetail>();
}
