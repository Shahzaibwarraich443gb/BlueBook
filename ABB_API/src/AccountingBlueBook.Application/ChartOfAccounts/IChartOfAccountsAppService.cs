using Abp.Application.Services.Dto;
using AccountingBlueBook.AccountTypes.Dto;
using AccountingBlueBook.AppServices.Invoices.dto;
using AccountingBlueBook.ChartOfAccounts.Dto;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using AccountingBlueBook.MainHeading.Dto;
using AccountingBlueBook.Users.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.ChartOfAccounts
{
    public interface IContactPersonTypeAppService
    {
        Task<List<ChartOfAccountDto>> GetAll();
        Task CreateOrEditChartOfAccount(CreateOrEditChartOfAccountInputDto input);
        Task<ChartOfAccountDto> GetChartOfAccounts(EntityDto input);
        Task<string> DeletChartOfAccounts(EntityDto input);
        Task<List<MainHeadDto>> GatAllMainHeadByAccountType(int accountTypeId);
        Task<List<AccountTypeDto>> GetAllAccountTypeByAccountNature(int accountNatureId);
        Task ChangeCoaBalance(List<InvoiceDetailDto> detailsInput, string type, int bankCoaId = 0, decimal PreviousAmount = 0);
        //Task<PagedResultDto<ChartOfAccountListViewDto>> GetAllChartOfAccountPagedList(PagedUserResultRequestDto input);
    }
}
