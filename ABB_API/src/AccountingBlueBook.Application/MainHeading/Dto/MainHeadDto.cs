using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.MainHeading.Dto
{
    public class MainHeadDto : EntityDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int? AccountTypeId { get; set; }
        public string AccountType { get; set; }
    }
}
