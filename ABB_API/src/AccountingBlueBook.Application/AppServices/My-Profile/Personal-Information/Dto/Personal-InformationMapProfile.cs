using AccountingBlueBook.AppServices.Emails.Dto;
using AccountingBlueBook.AppServices.paymentMethod;
using AccountingBlueBook.AppServices.Merchants.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.PersonalInformations.Dto
{
    public  class PersonalInformationMapProfile : Profile
    {
        public PersonalInformationMapProfile()
        {
            CreateMap<GeneralPersonalInformationDto,Employee>().ReverseMap();
            CreateMap<CreateOrEditPersonalInformationInputDto,Employee>().ReverseMap();
        }
    }
}
