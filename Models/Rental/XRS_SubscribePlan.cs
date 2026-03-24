using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XeniaRegistrationBackend.Models.Rental;

namespace XeniaRegistrationBackend.Models
{
    [Table("XRS_SubscribePlan", Schema = "dbo")]
    public class XRS_SubscribePlan
    {
        [Key]
        public int PlanId { get; set; }

        [Required, MaxLength(200)]
        public string PlanName { get; set; } = null!;

        [MaxLength(500)]
        public string? PlanDescription { get; set; }

        public int? PlanCreatedBy { get; set; }

        public DateTime PlanCreatedOn { get; set; } = DateTime.Now;

        public int? PlanModifiedBy { get; set; }

        public DateTime? PlanModifiedOn { get; set; }

        public bool PlanActive { get; set; } = true;
        public int PlanUsers { get; set; }
        public bool PlanIsAddOn { get; set; } 
        public ICollection<XRS_SubscribePlanDuration> PlanDurations { get; set; }
       = new List<XRS_SubscribePlanDuration>();
    }
}
