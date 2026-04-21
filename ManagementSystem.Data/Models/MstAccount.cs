using System;

namespace ManagementSystem.Data.Models;

[Table("MstAccount")]
[Index("AccountCode", Name = "IX_MstAccount", IsUnique = true)]
public partial class MstAccount
{
    [Key]
    public int AccountId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string AccountCode { get; set; } = string.Empty;

    [StringLength(300)]
    [Unicode(false)]
    public string AccountName { get; set; } = string.Empty;

    [StringLength(100)]
    [Unicode(false)]
    public string ModuleType { get; set; } = string.Empty;

    [StringLength(300)]
    [Unicode(false)]
    public string DebitNameFormat { get; set; } = string.Empty;

    [StringLength(300)]
    [Unicode(false)]
    public string CreditNameFormat { get; set; } = string.Empty;

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
