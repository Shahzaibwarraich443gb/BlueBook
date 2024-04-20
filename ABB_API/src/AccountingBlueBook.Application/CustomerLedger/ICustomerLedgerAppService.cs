using AccountingBlueBook.GeneralLedgers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.CustomerLedger
{
    internal interface ICustomerLedgerAppService
    {
        Task<List<GeneralLedgerOutputDto>> GetLedgerForTable(GetGeneralLedgerInputDto input);
    }
}
