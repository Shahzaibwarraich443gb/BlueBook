using Abp.Domain.Repositories;
using AccountingBlueBook.Entities.MainEntities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.LegalStatuses
{
    public class LegalStatusAppService : AccountingBlueBookAppServiceBase, ILegalStatusAppService
    {
        private readonly IRepository<LegalStatus, long> _legalStatusRepository;

        public LegalStatusAppService(IRepository<LegalStatus, long> legalStatusRepository)
        {
            _legalStatusRepository = legalStatusRepository;
        }
        public List<LegalStatus> GetAllLegalStatus()
        {
            return _legalStatusRepository.GetAll().ToList();
        }
    }
}
