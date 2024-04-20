using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;

namespace AccountingBlueBook.AppServices.SalesTaxes
{
    public class SalesTaxMapProfile : Profile
    {
        public SalesTaxMapProfile()
        {
            CreateMap<SalesTaxDto, SalesTax>();
        }
    }
}
