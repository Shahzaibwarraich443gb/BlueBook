using Abp.Application.Services.Dto;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.ChartOfAccounts.Dto
{
    public class CreateOrEditChartOfAccountInputDto: EntityDto
    {
        public AccountNature AccountNature { get; set; }
        public int? AccountTypeId { get; set; }
        public int? MainHeadId { get; set; }
        public long AccountCode { get; set; }
        public string AccountDescription { get; set; }
        public bool IsActive { get; set; }
    }
}
