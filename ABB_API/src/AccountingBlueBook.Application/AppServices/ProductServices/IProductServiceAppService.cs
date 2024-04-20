using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Languages.Dto;
using AccountingBlueBook.AppServices.ProductServices.Dto;
using AccountingBlueBook.ChartOfAccounts.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.ProductServices
{
    public interface IProductServiceAppService
    {
        Task<List<ProductServiceDto>> GetAll();
        Task CreateOrEdit(CreateOrEditProductServiceInputDto input);
        Task<ProductServiceDto> Get(EntityDto input);
        Task Delete(EntityDto input);
        Task<List<ChartOfAccountDto>> GetAllChartAccountIncome();
        Task<List<ChartOfAccountDto>> GetAllChartAccountExpense();
    }
}
