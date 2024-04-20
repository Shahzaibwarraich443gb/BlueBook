using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Ethnicities.Dto;
using AccountingBlueBook.AppServices.Languages.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Ethnicities
{
    public interface IEthnicitiesAppService
    {
        Task<List<EthnicityDto>> GetAll();
        Task CreateOrEdit(CreateOrEditEthnicityDto input);
        Task<EthnicityDto> Get(EntityDto input);
        Task Delete(EntityDto input);
    }
}
