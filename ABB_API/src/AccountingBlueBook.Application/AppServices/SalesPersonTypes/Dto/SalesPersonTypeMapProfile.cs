using AccountingBlueBook.AppServices.SalesPersonTypes.Dto;
using AccountingBlueBook.Entities.Main;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.SalesPersonTypes.Dto
{
    public class SalesPersonTypeMapProfile : Profile
    {
        public SalesPersonTypeMapProfile()
        {
            CreateMap<SalesPersonTypeDto, SalesPersonType>()
          
                .ForMember(x => x.Company, opt => opt.Ignore()).ReverseMap();
            CreateMap<CreateOrEditSalesPersonTypeDto, SalesPersonType>().ReverseMap();
        }
    }
}
