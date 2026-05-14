using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Catalog
{
    [Table("tblCompanySubscription", Schema = "dbo")]
    public class CT_CompanySubscription
    {
        [Key]
        public int SubId { get; set; }

        public int? PlanId { get; set; }

        public int? PlanDurationId { get; set; }

        public int? CompanyId { get; set; }


        public int? CustomerId { get; set; }

        public DateTime? SubscriptionStartDate { get; set; }

        public DateTime? SubscriptionEndDate { get; set; }

        public int? SubscriptionDays { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SubscriptionAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DealerAmount { get; set; }          

        //[Column(TypeName = "decimal(18,2)")]
        //public decimal? CustomAmount { get; set; }          

        [MaxLength(50)]
        public string? RateType { get; set; }               

        public int? SubscriptionUserCount { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; } = "ACTIVE";    

        public DateTime? CreatedOn { get; set; } = DateTime.Now;
    }
}
