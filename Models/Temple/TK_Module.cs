using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Temple
{
    [Table("TK_Module", Schema = "dbo")]
    public class TK_Module
    {
        [Key]
        public int ModuleId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ModuleName { get; set; } = null!;

        [MaxLength(500)]
        public string? ModuleDescription { get; set; }

        public bool ModuleActive { get; set; } = true;
    }
}
