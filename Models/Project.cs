using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models
{
    [Table("Projects", Schema = "dbo")]
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProjectName { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        [MaxLength(100)]
        public string? ModifiedBy { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
