using AutoMapper;
using AccountingBlueBook.Authorization.Users;

namespace AccountingBlueBook.Users.Dto
{
    public class UserMapProfile : Profile
    {
        public UserMapProfile()
        {
            CreateMap<UserDto, User>();
            CreateMap<UserDto, User>()
                .ForMember(x => x.Roles, opt => opt.Ignore())
                .ForMember(x => x.CreationTime, opt => opt.Ignore());

            CreateMap<User, CreateOrEditUserDto > ();
            CreateMap<CreateOrEditUserDto, User>();
            CreateMap<CreateOrEditUserDto, User>().ForMember(x => x.Roles, opt => opt.Ignore());
        }
    }
}
