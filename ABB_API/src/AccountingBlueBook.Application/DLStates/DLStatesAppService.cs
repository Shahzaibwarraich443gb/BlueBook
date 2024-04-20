using Abp.Domain.Repositories;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.DLStates
{
    public class DLStatesAppService : AccountingBlueBookAppServiceBase,IDLStatesAppService
    {
        private readonly IRepository<DLState> dlStateRepository;

        public DLStatesAppService(IRepository<DLState> dlStateRepository)
        {
            this.dlStateRepository = dlStateRepository;
        }
        public async Task<List<DLState>> GetDLState()
        {
			try
			{
                return await dlStateRepository.GetAll().ToListAsync();
			}
			catch (Exception ex)
			{

				throw;
			}
        }
    }
}
