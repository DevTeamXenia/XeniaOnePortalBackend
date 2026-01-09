using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models
{
    [Table("Users", Schema = "dbo")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Password { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }


        public int ModifiedBy { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
