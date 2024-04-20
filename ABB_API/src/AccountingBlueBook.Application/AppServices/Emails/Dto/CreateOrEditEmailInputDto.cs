using Abp.Application.Services.Dto;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Emails.Dto
{
    public class CreateOrEditEmailInputDto : EntityDto
    {
        public EmailType TypeEmail { get; set; }
        public string EmailAddress { get; set; }
        public bool IsPrimary { get; set; }
    }
}
