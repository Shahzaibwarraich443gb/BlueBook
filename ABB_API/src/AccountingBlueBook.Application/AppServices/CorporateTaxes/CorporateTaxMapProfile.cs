using AccountingBlueBook.Entities;
using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.CorporateTaxes
{
    public class CorporateTaxMapProfile : Profile
    {
        public CorporateTaxMapProfile()
        {
            CreateMap<CorporateTaxDto, CorporateTax>();
            CreateMap<CorporateTax, CorporateTaxDto>();
        }
    }
}
