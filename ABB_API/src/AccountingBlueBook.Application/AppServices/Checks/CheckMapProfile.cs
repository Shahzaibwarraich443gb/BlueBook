using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;

namespace AccountingBlueBook.AppServices.Checks
{
    public class CheckMapProfile: Profile
    {
        public CheckMapProfile()
        {
            CreateMap<CheckDto, Check>().ReverseMap();
            CreateMap<CheckSetupDto, CheckSetup>().ReverseMap();
        }

    }
}
