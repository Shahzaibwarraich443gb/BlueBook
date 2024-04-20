using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AccountingBlueBook.Roles.Dto;
using AccountingBlueBook.Users.Dto;

namespace AccountingBlueBook.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateOrEditUserDto, UserDto>
    {
        Task DeActivate(EntityDto<long> user);
        Task Activate(EntityDto<long> user);
        Task<ListResultDto<KeyValuePair<long, string>>> GetRoles(FilterRoleList input);
        Task ChangeLanguage(ChangeUserLanguageDto input);
        Task<CreateOrEditUserDto> GetUserForEdit(EntityDto<int> input);
        Task UpdateUser(CreateOrEditUserDto input);

        Task<bool> ChangePassword(ChangePasswordDto input);
    }
}
