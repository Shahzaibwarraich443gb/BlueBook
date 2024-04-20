using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Emails.Dto;
using AccountingBlueBook.AppServices.EntityTypes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.EntityTypes
{
     public  interface IGeneralEntityTypeAppService
    {
        Task<List<GeneralEntityTypeDto>> GetAll();
        Task CreateOrEdit(CreateOrEditGeneralEntityTypeInputDto input);
        Task<GeneralEntityTypeDto> Get(EntityDto input);
        Task Delete(EntityDto input);
    }
}
