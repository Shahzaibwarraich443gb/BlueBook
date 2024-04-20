using AccountingBlueBook.Entities.Main;
using AutoMapper;

namespace AccountingBlueBook.AppServices.ContactPersonTypes.Dto
{
    public class ContactPersonTypeMapProfile : Profile
    {
        public ContactPersonTypeMapProfile()
        {
            CreateMap<ContactPersonTypeDto, ContactPersonType>().ReverseMap();
            CreateMap<CreateOrEditContactPersonTypeInputDto, ContactPersonType>().ReverseMap();
        }
    }
}
