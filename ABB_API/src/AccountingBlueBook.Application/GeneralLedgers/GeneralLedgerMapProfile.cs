using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;

namespace AccountingBlueBook.GeneralLedgers
{
    public class GeneralLedgerMapProfile: Profile
    {
        public GeneralLedgerMapProfile()
        {
            CreateMap<GeneralLedgerDto, GeneralLedger>().ReverseMap();
        }
    }
}
