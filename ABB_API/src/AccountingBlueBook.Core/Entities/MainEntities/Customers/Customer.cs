using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingBlueBook.Entities.MainEntities.Customers
{
    [Table("Customers")]
    public class Customer : FullAuditedEntity, IMustHaveTenant
    {
        [Required]
        public string BussinessName { get; set; }
        public string TexId { get; set; }
        public DateTime? FiscalYearEnd { get; set; }
        public string Name { get; set; }
        public string BusinessDescription { get; set; }
        public string TaxId { get; set; }
        public string SocialSecurityNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? CustomerTypeId { get; set; }
        [ForeignKey("CustomerTypeId")]
        public virtual CustomerType CustomerType { get; set; }
        public string SSN { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string JobDescription { get; set; }
        public string DrivingLicense { get; set; }
        public DateTime? DLIssue { get; set; }
        public DateTime? DLExpiry { get; set; }
        public string DetailComment { get; set; }
        public string TodoListComment { get; set; }
        public string CRMComment { get; set; }
        public int? DLState { get; set; }
        public int Code { get; set; }
        public string? Comment { get; set; }
        public long? SpouseId { get; set; }
        [ForeignKey("SpouseId")]
        public virtual Spouse Spouse { get; set; }
        public string? DependentIds { get; set; }
        public virtual ICollection<Dependent> Dependent { get; set; }
        public int? SalesPersonTypeId { get; set; }
        [ForeignKey("SalesPersonTypeId")]
        public virtual SalesPersonType SalesPersonType { get; set; }

        public string LicenseComment { get; set; }

        public string passwordComment { get; set; }

        public int? SourceReferralTypeId { get; set; }
        [ForeignKey("SourceReferralTypeId")]
        public virtual SourceReferralType SourceReferralType { get; set; }

        public int? EthnicityId { get; set; }
        [ForeignKey("EthnicityId")]
        public virtual Ethnicity Ethnicity { get; set; }

        public int? LanguageId { get; set; }
        [ForeignKey("LanguageId")]
        public virtual Language Language { get; set; }
        public int? ContactPersonTypeId { get; set; }
        [ForeignKey("ContactPersonTypeId")]
        public virtual ContactPersonType ContactPersonType { get; set; }

        public virtual ICollection<CustomerPassword> CustomerPasswords { get; set; }


        //Old Cod
        public string Suffix { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string TaxRegNo { get; set; }
        public string Notes { get; set; }
        public string Password { get; set; }
        public DateTime? CompanyFormationDate { get; set; }
        public string StateUserName { get; set; }
        public string StateUserPassword { get; set; }
        public DateTime? FiscalYearStart { get; set; }
        public string ResponsiblePerson { get; set; }
        public long? EFTPS { get; set; }
        public string NysUsername { get; set; }
        public string NysPassword { get; set; }
        public string TaxFillingStatus { get; set; }
        public int? LegalStatus { get; set; }
        public int TenantId { get; set; }
        
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }


        public int? JobTitleId { get; set; }
        [ForeignKey("JobTitleId")]
        public virtual JobTitle JobTitle { get; set; }

        public int? GeneralEntityTypeId { get; set; }
        [ForeignKey("GeneralEntityTypeId")]
        public virtual GeneralEntityType GeneralEntityType { get; set; }
        public virtual IEnumerable<ContactInfo> ContactInfo { get; set; }
        public virtual IEnumerable<Address> Address { get; set; }
    }
}
