using Abp.Application.Services;
using AccountingBlueBook.Common.CommonLookupDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountingBlueBook.Common
{
    public interface ICommonAppService : IApplicationService
    {
        Task<List<KeyValuePair<int, string>>> CustomersLookups(CustomerLookupDto input);
    }
}
