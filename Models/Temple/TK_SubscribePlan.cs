using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Temple
{
    [Table("TK_SubscribePlan", Schema = "dbo")]
    public class TK_SubscribePlan
    {
        [Key]
        public int PlanId { get; set; }


        [Required, MaxLength(200)]
        public string PlanName { get; set; } = null!;

        [MaxLength(500)]
        public string? PlanDescription { get; set; }

        public int PlanUsers { get; set; }

        public bool PlanIsAddOn { get; set; }
        public int? PlanCreatedBy { get; set; }

        public DateTime PlanCreatedOn { get; set; } = DateTime.Now;

        public int? PlanModifiedBy { get; set; }

        public DateTime? PlanModifiedOn { get; set; }

        public bool PlanActive { get; set; } = true;

        public virtual ICollection<TK_SubscribePlanDuration> PlanDurations { get; set; }
      = new List<TK_SubscribePlanDuration>();
    }
}
