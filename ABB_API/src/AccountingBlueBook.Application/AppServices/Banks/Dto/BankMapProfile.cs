using AccountingBlueBook.AppServices.Adresses.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Banks.Dto
{
    public class BankMapProfile : Profile
    {
        public BankMapProfile()
        {
            CreateMap<BankAddressDto, BankAddress>().ReverseMap();
            CreateMap<CreateOrEditBankAddressDto, BankAddress>().ReverseMap();
            CreateMap<BankDto, Bank>().ReverseMap();
            CreateMap<CreateOrEditBankDto, Bank>().ReverseMap();
        }
    }
}
