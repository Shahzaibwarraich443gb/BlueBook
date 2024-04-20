using AccountingBlueBook.Entities;
using AccountingBlueBook.Entities.Main;
using AutoMapper;

namespace AccountingBlueBook.MainHeading.Dto
{
    public class MainHeadingMapProfile:Profile
    {
        public MainHeadingMapProfile()
        {
            CreateMap<MainHeadDto, MainHead>().ReverseMap();
            CreateMap<CreateOrEditMainHeadingInputDto, MainHead>().ReverseMap();
        }
    }
}
