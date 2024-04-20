using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Languages.Dto;
using AccountingBlueBook.AppServices.VendorContactInfos.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.VendorContactInfos
{
    public interface IVendorContactInfoAppService
    {
        Task<List<VendorContactInfoDto>> GetAll();
        Task<long> CreateOrEdit(CreateOrEditVendorContactInfoDto input);
        Task<VendorContactInfoDto> Get(EntityDto input);
        Task Delete(EntityDto input);
    }
}
