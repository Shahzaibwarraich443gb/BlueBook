using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Languages.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Languages
{
    public interface ILanguageAppService
    {
        Task<List<LanguageDto>> GetAll();
        Task CreateOrEdit(CreateOrEditLanguageInputDto input);
        Task<LanguageDto> Get(EntityDto input);
        Task Delete(EntityDto input);
    }
}
