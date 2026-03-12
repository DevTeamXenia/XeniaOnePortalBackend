using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Ticket
{
    [Table("TI_SubscribePlanDuration", Schema = "dbo")]
    public class TI_SubscribePlanDuration
    {
        public int PlanDurationId { get; set; }
        public int PlanId { get; set; }
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public TI_SubscribePlan Plan { get; set; } = null!;
    }
}
