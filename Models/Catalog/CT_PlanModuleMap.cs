using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Catalog
{
    [Table("tblPlanModuleMap", Schema = "dbo")]
    public class CT_PlanModuleMap
    {
        [Key]
        public int SubPlanId { get; set; }
        public int PlanId { get; set; }
        public int ModuleId { get; set; }
        public bool Active { get; set; } = true;
        public DateTime? CreatedOn { get; set; }
    }
}
