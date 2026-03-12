using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Token
{
    [Table("xtm_SubscribePlanDuration")]
    public class xtm_SubscribePlanDuration
    {
        [Key]
        public int PlanDurationId { get; set; }

        public int PlanId { get; set; }

        public int DurationDays { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [ForeignKey(nameof(PlanId))]
        public virtual xtm_SubscribePlan? SubscribePlan { get; set; }

    }
}
