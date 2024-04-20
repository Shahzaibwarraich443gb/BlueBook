using System.Threading.Tasks;
using Abp.Application.Services;
using AccountingBlueBook.Authorization.Accounts.Dto;

namespace AccountingBlueBook.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
