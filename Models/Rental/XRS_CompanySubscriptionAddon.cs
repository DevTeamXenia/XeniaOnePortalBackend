using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("XRS_CompanySubscriptionAddon")]
public class XRS_CompanySubscriptionAddon
{
    [Key]
    [Column("Id")]  // ← Database uses "Id", not "SubAddonId"
    public int Id { get; set; }

    [Column("MainPlanId")]
    public int MainPlanId { get; set; }

    [Column("PlanId")]
    public int PlanId { get; set; }

    [Column("CompanyId")]
    public int CompanyId { get; set; }

    [Column("Amount")]
    public decimal Amount { get; set; }

    [Column("DepCount")]  // ← Database uses "DepCount", not "UserCount"
    public int UserCount { get; set; }  // Keep property name as UserCount but map to DepCount

    [Column("Status")]
    public string Status { get; set; } = "ACTIVE";

    // These columns don't exist in your database, so remove them or make them nullable
    // [Column("CreatedOn")]
    // public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    // [Column("ModifiedOn")]
    // public DateTime? ModifiedOn { get; set; }
}
