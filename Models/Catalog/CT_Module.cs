using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Catalog
{
    [Table("tblModule", Schema = "dbo")]
    public class CT_Module
    {
        [Key]
        public int ModuleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ModuleName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ModuleDescription { get; set; }  // ? ADD THIS (matches Temple)

        public bool ModuleActive { get; set; } = true;  // ? RENAME from IsActive to ModuleActive

        // Navigation property for Plan-Module mapping (Catalog extra feature)
        //public virtual ICollection<CT_PlanModuleMap>? PlanModuleMaps { get; set; }
    }
}