using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Catalog
{
    [Table("tblCompanySubscriptionAddon", Schema = "dbo")]
    public class CT_CompanySubscriptionAddon
    {
        [Key]
        public int Id { get; set; }

        public int? MainPlanId { get; set; }    

        public int? PlanId { get; set; }      

        public int? CompanyId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }       

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DealerAmount { get; set; }  


        [MaxLength(50)]
        public string? RateType { get; set; }       

        public int? UserCount { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; } = "ACTIVE"; 

        public DateTime? CreatedOn { get; set; } = DateTime.Now;
    }
}
