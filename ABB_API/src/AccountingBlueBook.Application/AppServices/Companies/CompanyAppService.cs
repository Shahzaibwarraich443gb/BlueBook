using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using AccountingBlueBook.AppServices.Companies.Dto;
using AccountingBlueBook.AppServices.ProductCategories;
using AccountingBlueBook.AppServices.ProductCategories.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Companies
{
    public class CompanyAppService : AccountingBlueBookAppServiceBase, ICompanyAppService
    {
      
            private readonly IRepository<Company> _companyRepository;
            private IObjectMapper ObjectMapper;

            public CompanyAppService(IRepository<Company> companyRepository, IObjectMapper objectMapper)
            {
                _companyRepository = companyRepository;
                ObjectMapper = objectMapper;
            }

            public async Task CreateOrEdit(CreateOrEditCompanyDto input)
            {
                if (input.Id > 0)
                    await Update(input);
                else
                    await Cearte(input);
            }

            [UnitOfWork]
            private async Task Update(CreateOrEditCompanyDto input)
            {
                var company = await _companyRepository.FirstOrDefaultAsync((int)input.Id);
                company.Name = input.Name; 
                company.IsActive = input.IsActive;
                await _companyRepository.UpdateAsync(company);
            }

            [UnitOfWork]
            private async Task Cearte(CreateOrEditCompanyDto input)
            {
                var  company = ObjectMapper.Map<Company>(input);
            company.IsActive = true;
                await _companyRepository.InsertAsync(company);
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            public async Task Delete(EntityDto input)
            {
                var company = await _companyRepository.FirstOrDefaultAsync((int)input.Id);
                await _companyRepository.DeleteAsync(company);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            public async Task<CompanyDto> Get(EntityDto input)
            {
                var company = await _companyRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
                if (company != null)
                {
                    var obj = new CompanyDto();
                    obj.Id = company.Id;
                    obj.Name = company.Name;
                    obj.IsActive = company.IsActive;
                    return obj;
                }
                return null;
            }

            public async Task<List<CompanyDto>> GetAll()
            {
                var filteredQuery = _companyRepository.GetAll() .AsQueryable();

                var data = from o in filteredQuery
                           select new CompanyDto
                           {
                               Id = o.Id,
                               Name = o.Name,
                               IsActive = o.IsActive,
                               
                           };
                return await data.ToListAsync();
            }

        
    }
}
