
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace XeniaRegistrationBackend.Models
    {
        [Table("TI_CompanyLabel", Schema = "dbo")]
        public class TI_CompanyLabel
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public int CompanyId { get; set; }

            [Required]
            [MaxLength(100)]
            public string SettingKey { get; set; } = null!;

            [MaxLength(200)]
            public string? DisplayName { get; set; }

            [MaxLength(200)]
            public string? DisplayNameMa { get; set; }

            [MaxLength(200)]
            public string? DisplayNameTa { get; set; }

            [MaxLength(200)]
            public string? DisplayNameTe { get; set; }

            [MaxLength(200)]
            public string? DisplayNameKa { get; set; }

            [MaxLength(200)]
            public string? DisplayNameHi { get; set; }

            [MaxLength(200)]
            public string? DisplayNameMr { get; set; }

            [MaxLength(200)]
            public string? DisplayNamePa { get; set; }

            [MaxLength(200)]
            public string? DisplayNameSi { get; set; }

            public int? CreatedBy { get; set; }

            public DateTime? CreatedOn { get; set; }

            public int? ModifiedBy { get; set; }

            public DateTime? ModifiedOn { get; set; }

            public bool Active { get; set; } = true;
        }
    }
