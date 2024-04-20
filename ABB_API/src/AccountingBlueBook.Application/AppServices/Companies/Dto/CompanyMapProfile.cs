using AccountingBlueBook.AppServices.ProductCategories.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Companies.Dto
{
    public class CompanyMapProfile : Profile
    {
        public CompanyMapProfile()
        {
            CreateMap<CompanyDto, Company>();
            CreateMap<CreateOrEditCompanyDto, Company>();
        }
    }
}
 
