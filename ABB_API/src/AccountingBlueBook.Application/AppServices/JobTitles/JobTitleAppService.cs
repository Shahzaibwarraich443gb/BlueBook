using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using AccountingBlueBook.AppServices.EntityTypes;
using AccountingBlueBook.AppServices.EntityTypes.Dto;
using AccountingBlueBook.AppServices.JobTitles.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.JobTitles
{
    public  class JobTitleAppService : AccountingBlueBookAppServiceBase, IJobTitleAppService
    {
        private readonly IRepository<JobTitle> _jobTitleTypeRepository;
        private IObjectMapper ObjectMapper;

        public JobTitleAppService(IRepository<JobTitle> jobTitleTypeRepository, IObjectMapper objectMapper)
        {
            _jobTitleTypeRepository = jobTitleTypeRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEdit(CreateOrEditJobTitleInputDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Cearte(input);
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditJobTitleInputDto input)
        {
            var jobTitle = await _jobTitleTypeRepository.FirstOrDefaultAsync((int)input.Id);
            jobTitle.IsActive = input.IsActive;
            jobTitle.Name = input.Name;
            await _jobTitleTypeRepository.UpdateAsync(jobTitle);
        }

        [UnitOfWork]
        private async Task Cearte(CreateOrEditJobTitleInputDto input)
        {
            var jobTitle = ObjectMapper.Map<JobTitle>(input);
            jobTitle.IsActive = true;
            await _jobTitleTypeRepository.InsertAsync(jobTitle);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(EntityDto input)
        {
            var jobTitle = await _jobTitleTypeRepository.FirstOrDefaultAsync((int)input.Id); 
            await _jobTitleTypeRepository.DeleteAsync(jobTitle);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task<JobTitleDto> Get(EntityDto input)
        {
            var jobTitle = await _jobTitleTypeRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            if (jobTitle != null)
            {
                var obj = new JobTitleDto();
                obj.Id = jobTitle.Id;
                obj.Name = jobTitle.Name;
                obj.IsActive = jobTitle.IsActive;
                return obj;
            }
            return null;
        }

        public async Task<List<JobTitleDto>> GetAll()
        {
            var filteredQuery = _jobTitleTypeRepository.GetAll().AsQueryable();

            var data = from o in filteredQuery
                       select new JobTitleDto
                       {
                           Id = o.Id, 
                           Name = o.Name, 
                           IsActive = o.IsActive,
                       };
            return await data.ToListAsync();
        }

    }
}
