using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Temple
{
    [Table("TK_PlanModuleMap", Schema = "dbo")]
    public class TK_PlanModuleMap
    {
        [Key]
        public int SubPlanId { get; set; }

        public int PlanId { get; set; }

        public int ModuleId { get; set; }

        public bool Active { get; set; } = true;
    }
}
