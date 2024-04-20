using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Adresses.Dto;
using AccountingBlueBook.AppServices.Customers.Dto;
using AccountingBlueBook.AppServices.Emails.Dto;
using AccountingBlueBook.AppServices.Phones.Dto;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Venders.Dto
{
    public class CreateOrEditContectDTO : EntityDto
    {
        public VendorDto? VendorInfo { get; set; }
        public List<ContactInfoDto> ContactPersons { get; set; }
        public List<CustomerAddressDto> Address { get; set; }
    }
}
