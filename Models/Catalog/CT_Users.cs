using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Catalog
{

    [Table("tblUserSettings", Schema = "dbo")]
    public class CT_UserSetting
    {
        [Key]
        public int UserId { get; set; }

        public int? CompanyId { get; set; }

        public int? UserRoleId { get; set; }

        [MaxLength(500)]
        public string? UserName { get; set; }

        [MaxLength(500)]
        public string? Designation { get; set; }

        [MaxLength(4000)]
        public string? Address { get; set; }

        public DateTime? DOB { get; set; }

        [MaxLength(500)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Password { get; set; }

        [MaxLength(500)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Image { get; set; }

        public bool? Active { get; set; }

        public int? DefaultBranch { get; set; }

        // Navigation property
        [ForeignKey("CompanyId")]
        public virtual CT_tblCompany? Company { get; set; }
    }
}
