using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Adresses.Dto;
using AccountingBlueBook.AppServices.Banks.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Banks
{
    public interface IBankAppService
    {
        Task<List<BankDto>> GetAll();
      
       
        Task CreateOrEdit(CreateOrEditBankDto input);
        Task<BankDto> Get(EntityDto input);
        Task Delete(EntityDto input);
    }
}
