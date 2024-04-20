using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Adresses.Dto;
using AccountingBlueBook.AppServices.Emails.Dto;
using AccountingBlueBook.AppServices.Phones.Dto;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.SourceReferralTypes.Dto
{
    public class SourceReferralTypeDto : EntityDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public int EmailId { get; set; }
        public int PhoneId { get; set; }
        public int AddressId { get; set; }
        public int? CompanyId { get; set; }
        public string AddressData{ get; set; }
        public string PhoneData { get; set; }
        public string EmailAddress { get; set; }
        public string Company { get; set; }
    
        public CreateOrEditEmailInputDto Email { get; set; }
        public CreateOrEditPhoneDto Phone { get; set; }
        public CreateOrEditAddressDto Address { get; set; }
        
    }
}