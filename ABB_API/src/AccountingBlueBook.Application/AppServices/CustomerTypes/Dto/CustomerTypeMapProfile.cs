using AccountingBlueBook.AppServices.EntityTypes.Dto;
using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.CustomerTypes.Dto
{
    public  class CustomerTypeMapProfile : Profile
    {
        public CustomerTypeMapProfile()
        {
            CreateMap<CustomerTypeDto, CustomerType>().ReverseMap();
            CreateMap<CreateOrEditCustomerTypeInputDto, CustomerType>().ReverseMap();
        }
    }
}
