using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.ChartOfAccounts.Dto
{
    public class ChartOfAccountInputDto
    {
        public long IncomeAccountId { get; set; }
        public long ExpenseAccountId { get; set; }
        public long LiabilityAccountId { get; set; }
        public long AdvSaleTaxAccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal? LiabilitySaleTax { get; set; }
        public decimal? AdvancedSaleTax { get; set; }
        public decimal? Rate { get; set; }
    }
}
