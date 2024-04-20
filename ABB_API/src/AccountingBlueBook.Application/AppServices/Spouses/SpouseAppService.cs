using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using AccountingBlueBook.AppServices.ContactPersonTypes.Dto;
using AccountingBlueBook.AppServices.Languages.Dto;
using AccountingBlueBook.AppServices.Spouses.Dto;
using AccountingBlueBook.Entities.Main;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AccountingBlueBook.AppServices.Spouses
{
    public class SpouseAppService : AccountingBlueBookAppServiceBase, ISpouseAppService
    {
        private readonly IRepository<Spouse, long> _spouseRepository;

        public SpouseAppService(IRepository<Entities.Main.Spouse, long> spouseRepository)
        {
            _spouseRepository = spouseRepository;
        }

        public async Task CreateOrEdit(CreateOrEditSpouseDto input)
        {

            if (input.Id > 0)
                await Update(input);
            else
                await Create(input);
        }

        private async Task Update(CreateOrEditSpouseDto input)
        {
            var spouse = await _spouseRepository.FirstOrDefaultAsync((int)input.Id);
            //language.CompanyId = input.CompanyId;
            spouse.IsActive = input.IsActive;
            spouse.FirstName = input.FirstName;
            spouse.LastName = input.LastName;
            spouse.SpouseSuffix = input.SpouseSuffix;
            spouse.SSN = input.SSN;

            spouse.Email = input.Email;
            spouse.DateOfBirth = input.DateOfBirth.Date;
            spouse.EthnicityId = input.EthnicityId;
            spouse.LanguageId = input.LanguageId;
            spouse.SpouseJobDescription = input.SpouseJobDescription;
            spouse.JobTitleId = input.JobTitleId;

            await _spouseRepository.UpdateAsync(spouse);
        }

        private async Task Create(CreateOrEditSpouseDto input)
        {
            var spouse = ObjectMapper.Map<Spouse>(input);
            spouse.IsActive = true;
            await _spouseRepository.InsertAsync(spouse);
            await CurrentUnitOfWork.SaveChangesAsync();

        }

        public async Task Delete(EntityDto input)
        {
            var language = await _spouseRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            await _spouseRepository.DeleteAsync(language);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<SpouseDto> Get(EntityDto input)
        {
            var inputs = await _spouseRepository.GetAll().Include(x => x.Ethnicity).Include(x => x.JobTitle).Include(x => x.Company).FirstOrDefaultAsync(x => x.Id == input.Id);
            if (input != null)
            {
                var spouse = new SpouseDto();
                spouse.FirstName = inputs.FirstName;
                spouse.LastName = inputs.LastName;
                spouse.SpouseSuffix = inputs.SpouseSuffix;
                spouse.SSN = inputs.SSN;
                spouse.IsActive = spouse.IsActive;
                spouse.Id = input.Id;
                spouse.Email = inputs.Email;
                //spouse.DateOfBirth = inputs.DateOfBirth;
                spouse.EthnicityId = inputs.EthnicityId != null ? inputs.EthnicityId : null;
                spouse.LanguageId = inputs.LanguageId != null ? inputs.LanguageId : null;
                spouse.SpouseJobDescription = inputs.SpouseJobDescription;
                spouse.JobTitleId = inputs.JobTitleId != null ? inputs.JobTitleId : null;
                spouse.IsActive = inputs.IsActive;


                return spouse;
            }
            return null;

        }

        public async Task<List<SpouseDto>> GetAll()
        {
            var filteredQuery = _spouseRepository.GetAll().Include(data => data.Ethnicity)
                .Include(data => data.Language)
                .Include(data => data.JobTitle)
                .AsQueryable();

            var data = from o in filteredQuery
                       select new SpouseDto
                       {
                           Id = o.Id,
                           //CompanyId = o.CompanyId,
                           FirstName = o.FirstName,
                           LastName = o.LastName,
                           Email = o.Email,
                           IsActive = o.IsActive,
                           SSN = o.SSN,
                           //Ethnicity = o.Ethnicity,
                           //JobTitle = o.JobTitle,
                           //Language = o.Language == null ? null : o.Language,

                       };
            return await data.ToListAsync();
        }
    }
}