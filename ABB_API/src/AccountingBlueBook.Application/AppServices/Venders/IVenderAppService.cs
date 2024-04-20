using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.JobTitles.Dto;
using AccountingBlueBook.AppServices.Venders.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Venders
{
    public interface IVenderAppService
    {
        Task<List<VendorDto>> GetAll();
        Task<long> CreateOrEdit(CreateOrEditVenderDto input);
        Task<CreateOrEditVenderDto> GetVendor(EntityDto input);
        Task Delete(EntityDto input);
    }
}
