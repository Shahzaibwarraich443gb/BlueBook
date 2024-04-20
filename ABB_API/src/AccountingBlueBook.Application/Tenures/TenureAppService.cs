using Abp.Domain.Repositories;
using AccountingBlueBook.Entities.MainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Tenures
{
    public class TenureAppService : AccountingBlueBookAppServiceBase,ITenureAppService
    {
        private readonly IRepository<TenureForm, long> _tenureFormRepository;

        public TenureAppService(IRepository<TenureForm, long> tenureFormRepository)
        {
            _tenureFormRepository = tenureFormRepository;
        }
        public List<TenureForm> GetAllTenureForms()
        {
            return  _tenureFormRepository.GetAll().ToList();
        }
    }
}
