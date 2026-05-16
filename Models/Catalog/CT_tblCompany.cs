using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XeniaRegistrationBackend.Models.Catalog
{
    [Table("tblCompany", Schema = "dbo")]
    public class CT_tblCompany
    {
        [Key]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? TaxRegNo { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Phoneno1 { get; set; }
        public string? Phoneno2 { get; set; }
        public string? Email { get; set; }
        public string? Pincode { get; set; }
        public string? Website { get; set; }
        public string? Logo { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public DateTime? ValidityDate { get; set; }
        public string? Currency { get; set; }
        public string? OrderNoPrefix { get; set; }
        public string? OrderNoSuffix { get; set; }
        public bool? Active { get; set; }
        public int? DecimalValue { get; set; }
        public string? MajorCurrency { get; set; }
        public string? MinorCurrency { get; set; }
        public string? BranchLimit { get; set; }
        public string? FooterMessage { get; set; }
        public string? CompanyBrand { get; set; }
        public string? CustomerCareNo { get; set; }
        public string? CustomerCareEmail { get; set; }
        public string? TaxType { get; set; }
        //public bool? Barcode { get; set; }
        public string? SoldBy { get; set; }
        public string? CurrencySymbol { get; set; }
        public DateTime? CreatedDate { get; set; }
      
    }
}