using AccountingBlueBook.AccountTypes.Dto;
using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;

namespace AccountingBlueBook.AccountTypess.Dto
{
    public class AccountTypeMapProfile : Profile
    {
        public AccountTypeMapProfile()
        {
            CreateMap<AccountTypeDto, AccountType>();
            CreateMap<CreateOrEditAccountTypeInputDto, AccountType>();
        }
    }
}