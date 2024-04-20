using AccountingBlueBook.AppServices.Customers.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Venders.Dto
{
    public  class VenderMapProfile : Profile
    {
        public VenderMapProfile()
        {
            CreateMap<VendorDto, Vendor>().ReverseMap();
            CreateMap<CreateOrEditVenderDto, Vendor>().ReverseMap();
        }
    }
}

