using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.TrialBalance
{
    public interface ITrialBalanceAppService
    {
        Task<List<TrialBalanceDto>> GetTrialBalance();
    }
}
