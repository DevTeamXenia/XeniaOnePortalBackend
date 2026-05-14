using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Catalog
{
    [Table("tblSubscribePlan", Schema = "dbo")]
    public class CT_SubscribePlan
    {
        [Key]
        public int PlanId { get; set; }

        [MaxLength(500)]
        public string PlanName { get; set; }

        [MaxLength(4000)]
        public string? PlanDescription { get; set; }

        public int? PlanUsers { get; set; }

        public bool PlanIsAddOn { get; set; }

        public bool PlanActive { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<CT_SubscribePlanDuration> PlanDurations { get; set; }
            = new List<CT_SubscribePlanDuration>();
    }
}
