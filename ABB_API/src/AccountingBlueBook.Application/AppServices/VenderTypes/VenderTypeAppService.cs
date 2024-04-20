using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using AccountingBlueBook.AppServices.JobTitles.Dto;
using AccountingBlueBook.AppServices.JobTitles;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.ObjectMapping;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.AppServices.VenderTypes.Dto;
using Microsoft.EntityFrameworkCore;

namespace AccountingBlueBook.AppServices.VenderTypes
{
    public  class VenderTypeAppService : AccountingBlueBookAppServiceBase, IVenderTypeAppService
    {
        private readonly IRepository<VenderType> _venderTypeRepository;
        private IObjectMapper ObjectMapper;

        public VenderTypeAppService(IRepository<VenderType> venderTypeRepository, IObjectMapper objectMapper)
        {
            _venderTypeRepository = venderTypeRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEdit(CreateOrEditVenderTypeInputDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Cearte(input);
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditVenderTypeInputDto input)
        {
            var venderType = await _venderTypeRepository.FirstOrDefaultAsync((int)input.Id);
            venderType.IsActive = input.IsActive;
            venderType.Name = input.Name;
            await _venderTypeRepository.UpdateAsync(venderType);
        }

        [UnitOfWork]
        private async Task Cearte(CreateOrEditVenderTypeInputDto input)
        {
            var venderType = ObjectMapper.Map<VenderType>(input);
            venderType.IsActive = true;
            await _venderTypeRepository.InsertAsync(venderType);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(EntityDto input)
        {
            var jobTitle = await _venderTypeRepository.FirstOrDefaultAsync((int)input.Id);
            await _venderTypeRepository.DeleteAsync(jobTitle);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task<VenderTypeDto> Get(EntityDto input)
        {
            var jobTitle = await _venderTypeRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            if (jobTitle != null)
            {
                var obj = new VenderTypeDto();
                obj.Id = jobTitle.Id;
                obj.Name = jobTitle.Name;
                obj.IsActive = jobTitle.IsActive;
                return obj;
            }
            return null;
        }

        public async Task<List<VenderTypeDto>> GetAll()
        {
            var filteredQuery = _venderTypeRepository.GetAll().AsQueryable();

            var data = from o in filteredQuery
                       select new VenderTypeDto
                       {
                           Id = o.Id,
                           Name = o.Name,
                           IsActive = o.IsActive,
                       };
            return await data.ToListAsync();
        }

    }
}
