using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("TK_CompanySettings", Schema = "dbo")]
public class TK_CompanySettings
{
    [Key]
    public int CompanySettingsId { get; set; }

    public int CompanyId { get; set; }

    [Required, MaxLength(100)]
    public string KeyCode { get; set; } = null!;

    [Required]
    public string Value { get; set; } = null!;

    public bool Active { get; set; } = true;
}
