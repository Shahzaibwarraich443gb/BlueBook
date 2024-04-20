using Abp.Application.Services;
using AccountingBlueBook.MultiTenancy.Dto;

namespace AccountingBlueBook.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

