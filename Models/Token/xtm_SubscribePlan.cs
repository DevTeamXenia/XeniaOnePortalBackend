using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Token
{
    [Table("xtm_SubscribePlan")]
    public class xtm_SubscribePlan
    {
        [Key]
        public int PlanId { get; set; }

        public int CompanyId { get; set; }

        public required string PlanName { get; set; }

        public string? PlanDescription { get; set; }

        public int PlanDep { get; set; }

        public bool PlanIsAddOn { get; set; }

        public int PlanCreatedBy { get; set; }

        public DateTime PlanCreatedOn { get; set; }

        public int? PlanModifiedBy { get; set; }

        public DateTime? PlanModifiedOn { get; set; }

        public bool PlanActive { get; set; }
        public virtual ICollection<xtm_SubscribePlanDuration> PlanDurations { get; set; }
        = new List<xtm_SubscribePlanDuration>();
    }
}
