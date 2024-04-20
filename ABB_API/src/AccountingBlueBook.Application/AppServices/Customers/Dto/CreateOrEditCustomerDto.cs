using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Adresses.Dto;
using AccountingBlueBook.AppServices.Emails.Dto;
using AccountingBlueBook.AppServices.Phones.Dto;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using AccountingBlueBook.AppServices.Languages.Dto;
using AccountingBlueBook.AppServices.Ethnicities.Dto;
using AccountingBlueBook.AppServices.SalesPersonTypes.Dto;

namespace AccountingBlueBook.AppServices.Customers.Dto
{
    public class CreateOrEditCustomerDto : EntityDto
    {
        public CustomerInfoDto? CustomerInfo { get; set; }
        public DetailDto Detail { get; set; }
        public List<ContactInfoDto> ContactPersons { get; set; }
        public List<CustomerAddressDto> Address { get; set; }
        public List<FillingDetailsDto> FillingDetail { get; set; }
        public List<UserNamePasswordDto> UserPassword { get; set; }
        //Old Code
        public string BussinessName { get; set; }
        public string TaxId { get; set; }
        public string BusinessDescription { get; set; }
        public long? EFTPS { get; set; }
        public string? NysUserName { get; set; }
        public string? NysPassword { get; set; }
        public string Website { get; set; }
        public string? Comment { get; set; }
        public DateTime? FiscalYearStart { get; set; }
        public string? Email { get; set; }
        public DateTime? FiscalYearEnd { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string SocialSecurityNumber { get; set; }
        public string Name { get; set; }
        public string JobDescription { get; set; }
        public int? JobTitleId { get; set; }
        public string? DetailComment { get; set; }
        public string StateUserName { get; set; }
        public string StateUserPassword { get; set; }
        public int? SpouseId { get; set; }
        public int? LanguageId { get; set; }
        public int? EthnicityId { get; set; }
        public int? ContactPersonTypeId { get; set; }
        public int? SourceReferralTypeId { get; set; }
        public int? SalesPersonTypeId { get; set; }
        public int? GeneralEntityTypeId { get; set; }
        public IEnumerable<CreateOrEditAddressDto> Addresses { get; set; }
        public IEnumerable<CreateOrEditPhoneDto> PhoneNumbers { get; set; }
        public IEnumerable<CreateOrEditEmailInputDto> Emails { get; set; }
        public Spouse? spouse { get; set; }
    }

    public class DetailDto
    {
        public LanguageDto Language { get; set; }
        public EthnicityDto Ethnicity { get; set; }
        public SalesPersonTypeDto SalesPersonType { get; set; }
    }

    public class ContactInfoDto : AuditedEntityDto
    {
        public string ContactType { get; set; }
        public string Name { get; set; }
        public string NumberType { get; set; }
        public string Number { get; set; }
        public string EmailType { get; set; }
        public string Email { get; set; }
        public bool IsPrimary { get; set; }
        public string Fax { get; set; }
        public string EFax { get; set; }
        public string Website { get; set; }

    }

    public class CustomerAddressDto : AuditedEntityDto
    {
        public string Type { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string isPrimary { get; set; }
        public string Country { get; set; }
    }
    public class PagedResultInputRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public string filterType { get; set; }

    }
    public class FillingDetailsDto : AuditedEntityDto
    {
        public int? Type { get; set; }
        public int CorporationTaxType { get; set; }
        public DateTime FillingType { get; set; }
    }

    public class UserNamePasswordDto : AuditedEntityDto
    {
        public string Type { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
    }

    public class CustomerPasswordDto: AuditedEntityDto
    {
        public string Type { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

        public string? Description { get; set; }

        public int CustomerId { get; set; }

        public string? url { get; set; }

        public string? PasswordComment { get; set; }
    }

}
