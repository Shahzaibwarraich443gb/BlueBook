using Abp.Application.Services.Dto;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.TrialBalance
{
    public class TrialBalanceDto : EntityDto<long>
    {
        public long MainHeadId { get; set; }
        public string MainHeadName { get; set; }
        public long SubHeadId { get; set; }
        public string SubHeadName { get; set; }
        public double CreditAmount { get; set; }    
        public double DebitAmount { get; set; }
        public double OpeningBalance { get; set; } 
        public double Balance { get; set; }
        public string Type { get; set; }
        public double? TotalDebitAmount { get; set; }
        public double? TotalCreditAmount { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class TrialBalanceInputDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
