using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Phones.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Phones
{
    public interface IPhoneAppService
    {
        Task<List<PhoneDto>> GetAll();
        Task CreateOrEdit(CreateOrEditPhoneDto input);
        Task<PhoneDto> Get(EntityDto input);
        Task Delete(EntityDto input);
    }
}
