using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Catalog
{
    [Table("CT_CompanyLabel", Schema = "dbo")]
    public class CT_CompanyLabel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }

        public string? SettingKey { get; set; }
        public string? DisplayName { get; set; }
        public string? DisplayNameTa { get; set; }
        public string? DisplayNameMa { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
