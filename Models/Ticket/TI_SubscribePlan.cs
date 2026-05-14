using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XeniaRegistrationBackend.Models.Ticket;

namespace XeniaRegistrationBackend.Models
{
    [Table("TI_SubscribePlan", Schema = "dbo")]
    public class TI_SubscribePlan
    {
        [Key]
        public int PlanId { get; set; }

        [Required]
        [MaxLength(200)]
        public string PlanName { get; set; } = null!;

        [MaxLength(500)]
        public string? PlanDescription { get; set; }

        [Required]
        public bool PlanIsAddOn { get; set; }

        [Required]
        public int PlanUsers { get; set; }

        public int? PlanCreatedBy { get; set; }

        public DateTime? PlanCreatedOn { get; set; }

        public int? PlanModifiedBy { get; set; }

        public DateTime? PlanModifiedOn { get; set; }

        public bool PlanActive { get; set; } = true;

        public virtual ICollection<TI_SubscribePlanDuration> PlanDurations { get; set; }
  = new List<TI_SubscribePlanDuration>();

    }
}
