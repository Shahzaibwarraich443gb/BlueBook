using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.CustomerTypes.Dto;
using AccountingBlueBook.AppServices.EntityTypes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.CustomerTypes
{
    public  interface ICustomerTypesAppService 
    {
        Task<List<CustomerTypeDto>> GetAll();
        Task CreateOrEdit(CreateOrEditCustomerTypeInputDto input);
        Task<CustomerTypeDto> Get(EntityDto input);
        Task Delete(EntityDto input);

    }
}
