using System.Threading.Tasks;
using Abp.Application.Services;
using AccountingBlueBook.Sessions.Dto;

namespace AccountingBlueBook.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
