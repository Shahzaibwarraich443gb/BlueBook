using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Adresses.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Adresses
{
    public interface IAddressAppService
    {
        Task<List<AddressDto>> GetAll();
        Task CreateOrEdit(CreateOrEditAddressDto input);
        Task<AddressDto> Get(EntityDto input);
        Task Delete(EntityDto input);
    }
}
