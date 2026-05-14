using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Catalog
{
    [Table("tblSubscribePlanDuration", Schema = "dbo")]
    public class CT_SubscribePlanDuration
    {
        [Key]
        public int PlanDurationId { get; set; }

        public int? PlanId { get; set; }

        public int DurationDays { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }          // Standard Rate

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DealerPrice { get; set; }    // Dealer Rate

        //[Column(TypeName = "decimal(18,2)")]
        //public decimal? CustomPrice { get; set; }    // Custom Rate

        public bool IsActive { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        [ForeignKey(nameof(PlanId))]
        public virtual CT_SubscribePlan? SubscribePlan { get; set; }
    }
}
