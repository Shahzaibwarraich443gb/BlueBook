using AccountingBlueBook.Entities.Main;
using AutoMapper;

namespace AccountingBlueBook.ChartOfAccounts.Dto
{
    public class ChartOfAccountMapProfile : Profile
    {
        public ChartOfAccountMapProfile()
        {
            CreateMap<CreateOrEditChartOfAccountInputDto, ChartOfAccount>().ReverseMap();
            CreateMap<ChartOfAccount, ChartOfAccountDto>().ReverseMap();
            CreateMap<ChartOfAccountDto, ChartOfAccount>().ReverseMap();
            CreateMap<ChartOfAccountDto, ChartOfAccount>()
                .ForMember(x => x.AccountType, opt => opt.Ignore())
                .ForMember(x => x.AccountStatus, opt => opt.Ignore())
                .ForMember(x => x.MainHead, opt => opt.Ignore()).ReverseMap();
        }
    }
}