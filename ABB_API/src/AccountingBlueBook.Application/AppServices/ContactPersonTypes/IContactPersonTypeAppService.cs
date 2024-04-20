using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.ContactPersonTypes.Dto;
using AccountingBlueBook.ChartOfAccounts.Dto;
using AccountingBlueBook.MainHeading.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.ContactPersonTypes
{
    public interface IContactPersonTypeAppService 
    {
        Task<List<ContactPersonTypeDto>> GetAll();
        Task CreateOrEdit(CreateOrEditContactPersonTypeInputDto input);
        Task<ContactPersonTypeDto> Get(EntityDto input);
        Task Delete(EntityDto input);
    }
}
