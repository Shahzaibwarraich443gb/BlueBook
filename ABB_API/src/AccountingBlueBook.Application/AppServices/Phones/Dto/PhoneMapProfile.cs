using AccountingBlueBook.AppServices.Phones.Dto;
using AccountingBlueBook.Entities.Main;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Phones.Dto
{
    public class PhoneMapProfile : Profile
    {
        public PhoneMapProfile()
        {
            CreateMap<PhoneDto, Phone>().ReverseMap();
            CreateMap<CreateOrEditPhoneDto, Phone>().ReverseMap();
        }
    }
}
