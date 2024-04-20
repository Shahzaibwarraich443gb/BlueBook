using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using AccountingBlueBook.Common.CommonLookupDto;
using AccountingBlueBook.Entities.MainEntities.Customers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingBlueBook.Common
{
    public class CommonAppService : AccountingBlueBookAppServiceBase, ICommonAppService
    {
        private readonly IRepository<Customer> _customerRepository;

        public CommonAppService(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<List<KeyValuePair<int,string>>> CustomersLookups(CustomerLookupDto input)
        {
            var customers = await _customerRepository.GetAll()
                                                .WhereIf(!input.Filter.IsNullOrEmpty(), a => a.Name.StartsWith(input.Filter))
                                                .Select(a => new KeyValuePair<int, string>(a.Id, a.Name))
                                                .Take(100)
                                                .ToListAsync();
            return customers;
        }

    }
}
