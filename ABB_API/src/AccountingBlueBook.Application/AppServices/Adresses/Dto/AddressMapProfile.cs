using AccountingBlueBook.Entities.Main;
using AutoMapper;

namespace AccountingBlueBook.AppServices.Adresses.Dto
{
    public class AddressMapProfile : Profile
    {
        public AddressMapProfile()
        {
            CreateMap<AddressDto, Address>()
                .ForMember(x => x.Customer, opt => opt.Ignore()).ReverseMap();
            CreateMap<CreateOrEditAddressDto, Address>()
                .ForMember(x => x.Customer, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
