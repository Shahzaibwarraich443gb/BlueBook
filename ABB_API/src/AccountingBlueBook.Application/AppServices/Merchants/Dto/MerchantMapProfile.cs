using AccountingBlueBook.AppServices.Emails.Dto;
using AccountingBlueBook.AppServices.paymentMethod;
using AccountingBlueBook.AppServices.Merchants.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Merchants.Dto
{
    public  class PaymentMethodMapProfile : Profile
    {
        public PaymentMethodMapProfile()
        {
            CreateMap<GeneralMerchantDto,Merchant>().ReverseMap();
            CreateMap<CreateOrEditMerchantInputDto, PaymentMethod>().ReverseMap();
        }
    }
}
