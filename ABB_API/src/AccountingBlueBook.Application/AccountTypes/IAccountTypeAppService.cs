using Abp.Application.Services.Dto;
using AccountingBlueBook.AccountTypes.Dto;
using System.Threading.Tasks;

namespace AccountingBlueBook.AccountTypes
{
    public interface IAccountTypeAppService
    {
        Task CreateOrEditAccountType(CreateOrEditAccountTypeInputDto input);
        Task<AccountTypeDto> GetAccountTypes(EntityDto input);
        Task DeletAccountTypes(EntityDto input);
    }
}
