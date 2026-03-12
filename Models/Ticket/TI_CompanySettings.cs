using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models
{
    [Table("TK_CompanySettings", Schema = "dbo")]
    public class TI_CompanySettings
    {
        [Key]
        public int CompanySettingsId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        [MaxLength(100)]
        public string KeyCode { get; set; } = null!;

        [MaxLength(500)]
        public string? Value { get; set; }

        public bool Active { get; set; } = true;

     
    }
}
