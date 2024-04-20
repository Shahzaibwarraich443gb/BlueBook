using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Languages.Dto;
using AccountingBlueBook.AppServices.SalesPersonTypes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.SalesPersonTypes
{
    public interface ISalesPersonTypeAppService
    {
        Task<List<SalesPersonTypeDto>> GetAll();
        Task CreateOrEdit(CreateOrEditSalesPersonTypeDto input);
        Task<CreateOrEditSalesPersonTypeDto> Get(EntityDto input);
        Task Delete(EntityDto input);
    }
}
