using Abp.Application.Services.Dto;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Phones.Dto
{
    public class CreateOrEditPhoneDto : EntityDto
    {
        public PhoneType Type { get; set; }
        public string Number { get; set; }
        public bool IsPrimary { get; set; }
    }
}
