using AccountingBlueBook.Entities.Main;
using AutoMapper;

namespace AccountingBlueBook.AppServices.Ethnicities.Dto
{
    public class EthnicityMapProfile : Profile
    {
        public EthnicityMapProfile()
        {
            CreateMap<EthnicityDto, Ethnicity>().ReverseMap();
            CreateMap<CreateOrEditEthnicityDto, Ethnicity>().ReverseMap();
        }
    }
}
