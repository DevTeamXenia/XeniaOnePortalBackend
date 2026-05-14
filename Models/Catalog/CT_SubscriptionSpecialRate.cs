using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Catalog
{
    [Table("tblSubscriptionSpecialRate", Schema = "dbo")]
    public class CT_SubscriptionSpecialRate
    {
        [Key]
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int CompanyId { get; set; }
        public int PlanId { get; set; }
        public int PlanDurationId { get; set; }
        public int? AddonId { get; set; }
        public decimal? CustomRate { get; set; }  // ← ONLY ONE RATE FIELD
        public int? UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
