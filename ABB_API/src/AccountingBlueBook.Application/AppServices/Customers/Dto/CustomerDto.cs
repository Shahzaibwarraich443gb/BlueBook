using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Adresses.Dto;
using AccountingBlueBook.AppServices.ContactPersonTypes.Dto;
using AccountingBlueBook.AppServices.CustomerTypes.Dto;
using AccountingBlueBook.AppServices.Emails.Dto;
using AccountingBlueBook.AppServices.EntityTypes.Dto;
using AccountingBlueBook.AppServices.Phones.Dto;
using AccountingBlueBook.AppServices.SalesPersonTypes.Dto;
using AccountingBlueBook.AppServices.SourceReferralTypes.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Customers.Dto
{
    public class customerListDto
    {
        public CustomerDto CustomerList { get; set; }
    }
    public class CustomerDto : EntityDto
    {
        public string BussinessName { get; set; }
        public string TaxId { get; set; }
        public string BusinessDescription { get; set; }
        public string EFTPS { get; set; }
        public string Website { get; set; }
        public DateTime? FiscalYearStart { get; set; }
        public DateTime? FiscalYearEnd { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string SocialSecurityNumber { get; set; }
        public string Name { get; set; }
        public string JobDescription { get; set; }
        public string StateUserName { get; set; }
        public string StateUserPassword { get; set; }
        public int? SpouseId { get; set; }
        public long? RefCopmayId { get; set; }
        public Spouse Spouse { get; set; }
        public int? LanguageId { get; set; }
        public string CompanyName { get; set; }
        public Language Language { get; set; }
        public Ethnicity Ethnicity { get; set; }
        public ContactPersonTypeDto ContactPersonType { get; set; }
        public CustomerTypeDto CustomerType { get; set; }
        public SourceReferralTypeDto SourceReferralType { get; set; }
        public SalesPersonTypeDto SalesPersonType { get; set; }
        public GeneralEntityTypeDto EntityType { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string StateName { get; set; }
        public long StateCode { get; set; }
        public string DependentNames { get; set; }
        public string DependentRelations { get; set; }
    }
}
