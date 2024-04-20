using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Customers.Dto;
using AccountingBlueBook.AppServices.Spouses.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Spouses
{
    public interface ISpouseAppService
    {
        Task<List<SpouseDto>> GetAll();
        Task CreateOrEdit(CreateOrEditSpouseDto input);
        Task<SpouseDto> Get(EntityDto input);
        Task Delete(EntityDto input);
    }
}
