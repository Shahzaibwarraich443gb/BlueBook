using AccountingBlueBook.AppServices.Emails.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.EntityTypes.Dto
{
    public  class EntityTypeMapProfile : Profile
    {
        public EntityTypeMapProfile()
        {
            CreateMap<GeneralEntityTypeDto, GeneralEntityType>().ReverseMap();
            CreateMap<CreateOrEditGeneralEntityTypeInputDto, GeneralEntityType>().ReverseMap();
        }
    }
}
