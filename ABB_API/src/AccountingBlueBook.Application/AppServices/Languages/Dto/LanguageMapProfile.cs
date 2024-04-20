using AccountingBlueBook.Entities.Main;
using AutoMapper;

namespace AccountingBlueBook.AppServices.Languages.Dto
{
    public class LanguageMapProfile : Profile
    {
        public LanguageMapProfile()
        {
            CreateMap<LanguageDto, Language>().ReverseMap();
            CreateMap<CreateOrEditLanguageInputDto, Language>().ReverseMap();
        }
    }
}
