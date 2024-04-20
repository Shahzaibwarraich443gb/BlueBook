using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Emails.Dto;
using AccountingBlueBook.AppServices.entitytype.Dto;
using AccountingBlueBook.AppServices.Merchants.Dto;
using AccountingBlueBook.AppServices.paymentMethod.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Merchants
{
     public  interface IGeneralMerchantAppService 
    {
        Task<List<GeneralMerchantDto>> GetAll();
        //Task CreateOrEdit(CreateOrEditGeneralPaymentMethodInputDto input);
        //Task<GeneralPaymentMethodDto> Get(EntityDto input);

        //Task Delete(EntityDto input);
    }
}
