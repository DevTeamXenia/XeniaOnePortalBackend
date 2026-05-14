using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Catalog
{

    [Table("tblCompanySettings", Schema = "dbo")]
    public class CT_tblCompanySettings
    {
        [Key]
        public int CompanySettingsId { get; set; }
        public int CompanyId { get; set; }
        public string? KeyCode { get; set; }
        public string? Value { get; set; }
        public bool? Active { get; set; }
    }
}