using AccountingBlueBook.AppServices.Languages.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.ProductServices.Dto
{
    public  class ProductServiceMapProfile : Profile
    {
        public ProductServiceMapProfile()
        {
            CreateMap<ProductServiceDto, ProductService>().ReverseMap();
            CreateMap<CreateOrEditProductServiceInputDto, ProductService>().ReverseMap();
        }
    }
}
