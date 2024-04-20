using Abp.Application.Services.Dto;
using AccountingBlueBook.ChartOfAccounts.Dto;
using AccountingBlueBook.MainHeading.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Customers.Dto
{
    public interface CreateOrEditCustomerInputDto
    {
        Task<List<ChartOfAccountDto>> GetAll();
        Task CreateOrEdit(CreateOrEditChartOfAccountInputDto input);
        Task<ChartOfAccountDto> Get(EntityDto input);
        Task Delete(EntityDto input);
        Task<List<MainHeadDto>> GatAllMainHeadByAccountType(int accountTypeId);
    }
}
