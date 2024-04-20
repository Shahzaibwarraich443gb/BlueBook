using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.PersonalTaxes
{
    public class PersonalTaxMapProfile : Profile
    {
        public PersonalTaxMapProfile()
        {
            CreateMap<PersonalTaxDto, PersonalTax>();
            CreateMap<PersonalTax, PersonalTaxDto>();
        }
    }
}
