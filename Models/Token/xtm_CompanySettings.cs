namespace XeniaRegistrationBackend.Models.Token
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("xtm_CompanySettings", Schema = "dbo")]
    public class xtm_CompanySettings
    {
        [Key]
        [Column("CompSettingID")]
        public int CompSettingId { get; set; }

        [Column("CompanyID")]
        public int CompanyId { get; set; }

        [Column("CollectCustomerName")]
        public bool CollectCustomerName { get; set; }

        [Column("PrintCustomerName")]
        public bool PrintCustomerName { get; set; }

        [Column("CollectCustomerMobileNumber")]
        public bool CollectCustomerMobileNumber { get; set; }

        [Column("PrintCustomerMobileNumber")]
        public bool PrintCustomerMobileNumber { get; set; }

        [Column("IsCustomCall")]
        public bool IsCustomCall { get; set; }

        [Column("IsServiceEnable")]
        public bool IsServiceEnable { get; set; }
    }

}
