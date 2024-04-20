using AccountingBlueBook.AppServices.EntityTypes.Dto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.Main;

namespace AccountingBlueBook.AppServices.JobTitles.Dto
{
    public  class JobTitleMapProfile : Profile
    {
        public JobTitleMapProfile()
        {
            CreateMap<JobTitleDto, JobTitle>().ReverseMap();
            CreateMap<CreateOrEditJobTitleInputDto, JobTitle>().ReverseMap();
        }
    }
}
