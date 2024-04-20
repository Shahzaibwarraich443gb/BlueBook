using AccountingBlueBook.AppServices.JobTitles.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.VenderTypes.Dto
{
    public  class VenderTypeMapProfile : Profile
    {
        public VenderTypeMapProfile()
        {
            CreateMap<VenderTypeDto, VenderType>().ReverseMap();
            CreateMap<CreateOrEditVenderTypeInputDto, VenderType>().ReverseMap();
        }
    }
}

