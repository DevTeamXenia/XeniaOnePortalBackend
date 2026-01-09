using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("TK_CompanyLabel", Schema = "dbo")]
public class TK_CompanyLabel
{
    [Key]
    public int Id { get; set; }

    public int CompanyId { get; set; }

    [Required, MaxLength(100)]
    public string SettingKey { get; set; } = null!;

    public string? DisplayName { get; set; }
    public string? DisplayNameMa { get; set; }
    public string? DisplayNameTa { get; set; }
    public string? DisplayNameTe { get; set; }
    public string? DisplayNameKa { get; set; }
    public string? DisplayNameHi { get; set; }
    public string? DisplayNameMr { get; set; }
    public string? DisplayNamePa { get; set; }
    public string? DisplayNameSi { get; set; }

    public int? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public int? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }

    public bool Active { get; set; } = true;
}
