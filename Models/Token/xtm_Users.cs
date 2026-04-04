using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourProjectNamespace.Models
{
    [Table("xtm_Users")]
    public class XtmUser
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        public int CompanyID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Password { get; set; } = null!;

        [Required]
        public bool TokenResetAllowed { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserType { get; set; } = null!;

        [Required]
        public bool Status { get; set; }
    }
}