using AccountingBlueBook.AppServices.Customers.Dto;
using AccountingBlueBook.Entities.Main;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Spouses.Dto
{
    public  class SpouseMapProfileProfile : Profile
    {
        public SpouseMapProfileProfile()
        {
            CreateMap<SpouseDto, Spouse>().ReverseMap();
            CreateMap<CreateOrEditSpouseDto, Spouse>().ReverseMap();
        }
    }
}
