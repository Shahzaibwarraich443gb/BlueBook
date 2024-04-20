using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Adresses.Dto;
using AccountingBlueBook.AppServices.Emails.Dto;
using AccountingBlueBook.AppServices.Phones.Dto;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.SourceReferralTypes.Dto
{
    public class CreateOrEditSourceReferralTypeDto : EntityDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public CreateOrEditEmailInputDto Email { get; set; }
        public CreateOrEditPhoneDto Phone { get; set; }
        public CreateOrEditAddressDto Address { get; set; }
        public int ? CompanyId { get; set; }
    }
}
