using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using AccountingBlueBook.AppServices.Ethnicities.Dto;
using AccountingBlueBook.AppServices.Languages.Dto;
using AccountingBlueBook.Entities.Main;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Ethnicities
{
    public class EthnicitiesAppService : AccountingBlueBookAppServiceBase, IEthnicitiesAppService
    {
        private readonly IRepository<Ethnicity> _ethnicityRepository;
        private IObjectMapper ObjectMapper;

        public EthnicitiesAppService(IRepository<Ethnicity> ethnicityRepository, IObjectMapper objectMapper)
        {
            _ethnicityRepository = ethnicityRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEdit(CreateOrEditEthnicityDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Cearte(input);
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditEthnicityDto input)
        {
            var ethnicity = await _ethnicityRepository.FirstOrDefaultAsync((int)input.Id);
            ethnicity.IsActive = input.IsActive;
            ethnicity.Name = input.Name;
            ethnicity.Descripition = input.Descripition;
            await _ethnicityRepository.UpdateAsync(ethnicity);
        }

        [UnitOfWork]
        private async Task Cearte(CreateOrEditEthnicityDto input)
        {
            var ethnicity = ObjectMapper.Map<Ethnicity>(input);
            ethnicity.IsActive = true;
            await _ethnicityRepository.InsertAsync(ethnicity);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(EntityDto input)
        {
            var ethnicity = await _ethnicityRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            await _ethnicityRepository.DeleteAsync(ethnicity);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<EthnicityDto> Get(EntityDto input)
        {
            var ethnicity = await _ethnicityRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id); 
            if (ethnicity != null)
            {
                var obj = new EthnicityDto();
                obj.Id = ethnicity.Id;
                obj.Name = ethnicity.Name;
                obj.IsActive= ethnicity.IsActive;
                obj.Descripition = ethnicity.Descripition; 
                obj.CompanyId = ethnicity.Company != null ? ethnicity.Company.Id : null;
                return obj;
            }
            return null;
 
        }

        public async Task<List<EthnicityDto>> GetAll()
        {
            // todo:    CompanyId 
            var filteredQuery = _ethnicityRepository.GetAll().Include(data=>data.Company).AsQueryable();

            var data = from o in filteredQuery
                       select new EthnicityDto
                       {
                           Id = o.Id,
                           //CompanyId = o.CompanyId,
                           Name = o.Name,
                           Descripition = o.Descripition,
                           IsActive= o.IsActive,
                           Company = o.Company == null ? null : o.Company
                       };
            return await data.ToListAsync();
        }
    }
}