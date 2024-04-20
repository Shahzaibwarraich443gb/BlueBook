using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.EntityTypes.Dto;
using AccountingBlueBook.AppServices.JobTitles.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.JobTitles
{
    public  interface IJobTitleAppService 
    { 
    Task<List<JobTitleDto>> GetAll();
    Task CreateOrEdit(CreateOrEditJobTitleInputDto input);
    Task<JobTitleDto> Get(EntityDto input);
    Task Delete(EntityDto input);
    
    }
}
