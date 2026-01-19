using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Temple
{
    [Table("TK_Company", Schema = "dbo")]
    public class TK_Company
    {
        [Key]
        public int CompanyId { get; set; }

        [Required, MaxLength(200)]
        public string CompanyName { get; set; } = null!;

        [MaxLength(500)]
        public string? CompanyAddress { get; set; }

        [MaxLength(20)]
        public string? CompanyPhone1 { get; set; }

        [MaxLength(20)]
        public string? CompanyPhone2 { get; set; }

        [MaxLength(100)]
        public string? CompanyRegNo { get; set; }

        [MaxLength(50)]
        public string? CompanyType { get; set; }

        [MaxLength(100)]
        public string? DistrictName { get; set; }

        [MaxLength(100)]
        public string? StateName { get; set; }

        [MaxLength(20)]
        public string? IFSCCode { get; set; }

        [MaxLength(200)]
        public string? CompanyToken { get; set; }

        public DateTime CompanyCreatedOn { get; set; } = DateTime.Now;
        public int? CompanyCreatedBy { get; set; }

        public DateTime? CompanyModifiedOn { get; set; }
        public int? CompanyModifiedBy { get; set; }

        public bool CompanyActive { get; set; } = true;
    }
}
