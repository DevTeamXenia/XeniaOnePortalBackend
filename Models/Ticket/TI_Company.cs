using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("TI_Company", Schema = "dbo")]
public class TI_Company
{
    [Key]
    public int CompanyId { get; set; }

    [Required]
    [MaxLength(50)]
    public string CompanyName { get; set; }

    public string CompanyAddress { get; set; }

    [MaxLength(12)]
    public string CompanyPhone1 { get; set; }

    [MaxLength(12)]
    public string CompanyPhone2 { get; set; }

    [MaxLength(50)]
    public string CompanyRegNo { get; set; }

    [MaxLength(50)]
    public string? CompanyType { get; set; }

    [MaxLength(100)]
    public string? DistrictName { get; set; }

    [MaxLength(50)]
    public string StateName { get; set; }

    [MaxLength(50)]
    public string? IFSCCode { get; set; }

    [MaxLength(50)]
    public string? CompanyToken { get; set; }

    public DateTime? CompanyCreatedOn { get; set; }

    public int? CompanyCreatedBy { get; set; }

    public DateTime? CompanyModifiedOn { get; set; }

    public int? CompanyModifiedBy { get; set; }

    public bool? CompanyActive { get; set; }
}
