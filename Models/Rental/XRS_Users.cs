using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Rental
{
    [Table("XRS_Users", Schema = "dbo")]
    public class XRS_Users
    {
        [Key]
        [Column("userID")]  // ← Map to database column
        public int UserID { get; set; }

        [Column("companyID")]  // ← Map to database column
        public int CompanyID { get; set; }

        [Column("userType")]  // ← Map to database column
        public int? UserType { get; set; }

        [MaxLength(50)]
        [Column("userName")]  // ← Map to database column
        public string? UserName { get; set; }

        [MaxLength(50)]
        [Column("password")]  // ← Map to database column
        public string? Password { get; set; }

        [MaxLength(50)]
        [Column("phone")]  // ← Map to database column
        public string? Phone { get; set; }

        [MaxLength(50)]
        [Column("email")]  // ← Map to database column
        public string? Email { get; set; }

        [Column("createdDate")]  // ← Map to database column
        public DateTime? CreatedDate { get; set; }

        [Column("modifiedDate")]  // ← Map to database column
        public DateTime? ModifiedDate { get; set; }

        [Column("isActive")]  // ← Map to database column
        public bool IsActive { get; set; }
    }
}