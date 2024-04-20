using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.JobTitles.Dto;
using AccountingBlueBook.AppServices.VenderTypes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.VenderTypes
{
    public interface IVenderTypeAppService
    {
        Task<List<VenderTypeDto>> GetAll();
        Task CreateOrEdit(CreateOrEditVenderTypeInputDto input);
        Task<VenderTypeDto> Get(EntityDto input);
        Task Delete(EntityDto input);
    }
}
