using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.ChartOfAccounts.Dto
{
    public class ChartOfAccountListViewDto : AuditedEntityDto
    {
        public string AccountNature { get; set; }
        public int? AccountNatureId { get; set; }
        public int? AccountTypeId { get; set; }
        public string AccountType { get; set; }
        public int? MainHeadId { get; set; }
        public string MainHead { get; set; }
        public string AccountDescription { get; set; }
        public bool? IsActive { get; set; }

        public IEnumerable<ChartOfAccountListViewDto> RelevantAccounts { get; set; }
    }
}
