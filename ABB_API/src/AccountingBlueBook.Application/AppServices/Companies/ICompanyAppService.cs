using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Companies.Dto;
using AccountingBlueBook.AppServices.ProductCategories.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Companies
{
    public  interface ICompanyAppService
    {
        Task<List<CompanyDto>> GetAll();
        Task CreateOrEdit(CreateOrEditCompanyDto input);
        Task<CompanyDto> Get(EntityDto input);
        Task Delete(EntityDto input);
    }
}
