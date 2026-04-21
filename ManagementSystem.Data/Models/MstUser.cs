using System;

namespace ManagementSystem.Data.Models;

[Table("MstUser")]
public partial class MstUser
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string UserId { get; set; } = string.Empty;

    [StringLength(100)]
    [Unicode(false)]
    public string UserName { get; set; } = string.Empty;

    [StringLength(500)]
    [Unicode(false)]
    public string UserPassword { get; set; } = string.Empty;

    [StringLength(500)]
    [Unicode(false)]
    public string Salt { get; set; } = string.Empty;

    [StringLength(8000)]
    [Unicode(false)]
    public string AccessList { get; set; } = string.Empty;

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
}
