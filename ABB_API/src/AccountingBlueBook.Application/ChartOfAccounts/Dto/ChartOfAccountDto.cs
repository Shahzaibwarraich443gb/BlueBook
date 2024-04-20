using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.ChartOfAccounts.Dto
{
    public class ChartOfAccountDto : FullAuditedEntityDto
    {
        public long ChartOfAccountId { get; set; }
        public string AccountNature { get; set; }
        public int? AccountNatureId { get; set; }
        public long? AccountTypeCode { get; set; }
        public int? AccountTypeId { get; set; }
        public string AccountType { get; set; }
        public int? MainHeadId { get; set; }

        public decimal? Balance { get; set; }
        public string MainHead { get; set; }
        public long? MainHeadCode { get; set; }
        public string AccountDescription { get; set; }
        public long? AccountCode { get; set; }
        public bool ? IsActive { get; set; }

    }

}
