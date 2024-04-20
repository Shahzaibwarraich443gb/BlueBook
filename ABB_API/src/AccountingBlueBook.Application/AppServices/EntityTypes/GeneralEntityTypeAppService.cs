using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using AccountingBlueBook.AppServices.EntityTypes.Dto;
using AccountingBlueBook.AppServices.Languages.Dto;
using AccountingBlueBook.AppServices.SourceReferralTypes;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.EntityTypes
{
    public  class GeneralEntityTypeAppService : AccountingBlueBookAppServiceBase, IGeneralEntityTypeAppService
    {
        private readonly IRepository<GeneralEntityType> _generalEntityTypeRepository;
        private IObjectMapper ObjectMapper;

        public GeneralEntityTypeAppService(IRepository<GeneralEntityType>  generalEntityTypeRepository, IObjectMapper objectMapper)
        {
            _generalEntityTypeRepository = generalEntityTypeRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEdit(CreateOrEditGeneralEntityTypeInputDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Cearte(input);
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditGeneralEntityTypeInputDto input)
        {
            var generalEntityType = await _generalEntityTypeRepository.FirstOrDefaultAsync((int)input.Id);
            //language.CompanyId = input.CompanyId;
            generalEntityType.Name = input.Name;
            generalEntityType.IsActive= input.IsActive; 
            await _generalEntityTypeRepository.UpdateAsync(generalEntityType);
        }

        [UnitOfWork]
        private async Task Cearte(CreateOrEditGeneralEntityTypeInputDto input)
        {
            var createOrEditGeneralEntityType = ObjectMapper.Map<GeneralEntityType>(input);
            createOrEditGeneralEntityType.IsActive = true;
            await _generalEntityTypeRepository.InsertAsync(createOrEditGeneralEntityType);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(EntityDto input)
        {
            var generalEntityType = await _generalEntityTypeRepository.FirstOrDefaultAsync((int)input.Id); 
            await _generalEntityTypeRepository.DeleteAsync(generalEntityType);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task<GeneralEntityTypeDto> Get(EntityDto input)
        {
            var generalEntityType = await _generalEntityTypeRepository.GetAll() .FirstOrDefaultAsync(x => x.Id == input.Id);
            if (generalEntityType != null)
            { 
                var obj = new GeneralEntityTypeDto();
                obj.Id = generalEntityType.Id;
                obj.Name = generalEntityType.Name;
                obj.IsActive = generalEntityType.IsActive; 
                return obj;
            } 
            return null;

        }

        public async Task<List<GeneralEntityTypeDto>> GetAll()
        {
            // todo :   CompanyId  
            var filteredQuery = _generalEntityTypeRepository.GetAll().AsQueryable();

            var data = from o in filteredQuery
                       select new GeneralEntityTypeDto
                       {
                           Id = o.Id,
                           IsActive = o.IsActive,
                           //CompanyId = o.CompanyId,
                           Name = o.Name,
                        
                       };
            return await data.ToListAsync();
        }

       
    }
}
