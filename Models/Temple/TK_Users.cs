using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Temple
{
    [Table("TK_Users", Schema = "dbo")]
    public class TK_Users
    {
        [Key]
        [Column("userId")]  
        public int UserId { get; set; }

        [Column("companyId")] 
        public int CompanyId { get; set; }

        [Required, MaxLength(100)]
        [Column("userName")]  
        public string UserName { get; set; } = null!;

        [Required, MaxLength(50)]
        [Column("password")]  
        public string Password { get; set; } = null!;

        [MaxLength(50)]
        [Column("userType")] 
        public string? UserType { get; set; }

        [Column("userCreatedOn")]  
        public DateTime? UserCreatedOn { get; set; }

        [Column("userCreatedBy")]  
        public int? UserCreatedBy { get; set; }

        [Column("userStatus")]  
        public bool? UserStatus { get; set; }
    }
}