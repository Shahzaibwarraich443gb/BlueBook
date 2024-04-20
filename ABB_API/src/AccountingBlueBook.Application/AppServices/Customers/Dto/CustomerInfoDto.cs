using AccountingBlueBook.AppServices.CustomerTypes.Dto;
using AccountingBlueBook.AppServices.Dependents.Dto;
using AccountingBlueBook.AppServices.SourceReferralTypes.Dto;
using AccountingBlueBook.AppServices.Spouses.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AccountingBlueBook.AppServices.Customers.Dto
{

    public class CustomerInfoDto 
    {
        public int? Id { get; set; }
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
        public virtual CustomerTypeDto CustomerType { get; set; }
        public int? customerTypeId { get; set; }
        public List<int>? dependentId { get; set; }
        public string? Comment { get; set; }
        public string SSN { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string JobDescription { get; set; }
        public string DrivingLicense { get; set; }
        public DateTime? DLIssue { get; set; }
        public DateTime? DLExpiry { get; set; }
        public int? DLState { get; set; }
        public int Code { get; set; }
        public virtual SpouseDto Spouse { get; set; }
        public virtual ICollection<DependentDto> Dependent { get; set; }
        public SourceReferralTypeDto SourceReferralType { get; set; }
    }
}
