using AccountingBlueBook.Entities.Main;
using AutoMapper;

namespace AccountingBlueBook.AppServices.SourceReferralTypes.Dto
{
    public class SourceReferralMapProfile : Profile
    {
        public SourceReferralMapProfile()
        {
            CreateMap<SourceReferralTypeDto, SourceReferralType>().ReverseMap();
            CreateMap<CreateOrEditSourceReferralTypeDto, SourceReferralType>().ReverseMap();
        }
    }
}
