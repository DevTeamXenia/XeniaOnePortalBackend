using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Token
{
    [Table("xtm_Company")]
    public class xtm_Company
    {
        [Key]
        public int CompanyID { get; set; }

        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = null!;

        public bool? Status { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }


        [MaxLength(150)]
        public string? Email { get; set; }

        //[MaxLength(100)]
        //public string? UserName { get; set; }

        //[MaxLength(200)]
        //public string? Password { get; set; }
    }
}
