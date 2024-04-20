using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using AccountingBlueBook.AppServices.ContactPersonTypes.Dto;
using AccountingBlueBook.AppServices.Languages.Dto;
using AccountingBlueBook.Entities.Main;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Languages
{
    [AbpAuthorize]
    public class LanguageAppService : AccountingBlueBookAppServiceBase, ILanguageAppService
    {
        private readonly IRepository<Language> _languageRepository;
        private IObjectMapper ObjectMapper;

        public LanguageAppService(IRepository<Language> languageRepository, IObjectMapper objectMapper)
        {
            _languageRepository = languageRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEdit(CreateOrEditLanguageInputDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Cearte(input);
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditLanguageInputDto input)
        {
            var language = await _languageRepository.FirstOrDefaultAsync((int)input.Id);
            //language.CompanyId = input.CompanyId;
            language.Name = input.Name;
            language.IsActive = input.IsActive;
            language.Description = input.Description;
            language.Code = input.Code; 
            await _languageRepository.UpdateAsync(language);
        }

        [UnitOfWork]
        private async Task Cearte(CreateOrEditLanguageInputDto input)
        {
            var language = ObjectMapper.Map<Language>(input);
            language.IsActive = true;  
            await _languageRepository.InsertAsync(language);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(EntityDto input)
        {
            var language = await _languageRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            await _languageRepository.DeleteAsync(language);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<LanguageDto> Get(EntityDto input)
        {
            var language = await _languageRepository.GetAll().Include(x => x.Company).FirstOrDefaultAsync(x => x.Id == input.Id);
            if (language != null)
            { 
                var obj = new LanguageDto();
                obj.Id = language.Id;
                obj.Name = language.Name;
;               obj.IsActive = language.IsActive; 
                obj.Code = language.Code;
                obj.Description = language.Description;
                obj.CompanyName = language.Company != null ? language.Company.Name : null;
                obj.CompanyId = language.Company != null ? language.Company.Id : null;
                return obj;
            } 
            return null;
           
        }

        public async Task<List<LanguageDto>> GetAll()
        {
            //todo:  CompanyId 
            var filteredQuery = _languageRepository.GetAll().AsQueryable();

            
            var data = from o in filteredQuery
                       select new LanguageDto
                       {
                           Id = o.Id,
                           IsActive = o.IsActive,
                           Name = o.Name,
                           Description = o.Description,
                           Code = o.Code,   
                           CompanyName = o.Company == null ? null : o.Company.Name
                       };
            return await data.ToListAsync();
        }
    }
}