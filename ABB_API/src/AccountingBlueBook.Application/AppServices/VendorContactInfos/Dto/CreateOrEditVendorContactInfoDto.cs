using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingBlueBook.AppServices.VendorContactInfos.Dto
{
    public class CreateOrEditVendorContactInfoDto : EntityDto
    {
        public string ContactPersonName { get; set; }
        public string ContactTypeName { get; set; }
        public string Fax { get; set; }
        public int? EmailTypeId { get; set; }
        public string EFax { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string WebSite { get; set; }
        public bool Primary { get; set; }
        public int? ContactPersonTypeId { get; set; }
        public ContactPersonType ContactPersonType { get; set; }
        public int? ContactTypeId { get; set; }
        public int? VendorId { get; set; }
    }
    public class CreateOrEditVendorAddressDto : EntityDto
    {
        public string CompleteAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }
        public string Fax { get; set; }
        public string Type { get; set; }
        public bool IsPrimary { get; set; }
        public int? VendorId { get; set; }
        [ForeignKey("VendorId")]
        public Vendor vendor { get; set; }
    }
}
