using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Temple
{
    [Table("TK_Users", Schema = "dbo")]
    public class TK_Users
    {
        [Key]
        [Column("userId")]  // ? Map to database column
        public int UserId { get; set; }

        [Column("companyId")]  // ? Map to database column
        public int CompanyId { get; set; }

        [Required, MaxLength(100)]
        [Column("userName")]  // ? Map to database column
        public string UserName { get; set; } = null!;

        [Required, MaxLength(50)]
        [Column("password")]  // ? Map to database column
        public string Password { get; set; } = null!;

        [MaxLength(50)]
        [Column("userType")]  // ? Map to database column
        public string? UserType { get; set; }

        [Column("userToken")]  // ? Map to database column
        public string? UserToken { get; set; }

        [Column("userCreatedOn")]  // ? Map to database column
        public DateTime? UserCreatedOn { get; set; }

        [Column("userCreatedBy")]  // ? Map to database column
        public int? UserCreatedBy { get; set; }

        [Column("userStatus")]  // ? Map to database column
        public bool? UserStatus { get; set; }
    }
}