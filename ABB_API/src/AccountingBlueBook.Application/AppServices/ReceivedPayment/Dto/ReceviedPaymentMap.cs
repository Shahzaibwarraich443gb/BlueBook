using AccountingBlueBook.Entities.MainEntities.Invoices;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.ReceivedPayment.Dto
{
    public class ReceviedPaymentMap : Profile
    {
        public ReceviedPaymentMap()
        {
            CreateMap<InvoiceDetail, ReceivedPaymentDto>().ReverseMap();
            CreateMap<Invoice, ReceivedPaymentDto>().ReverseMap();
        }
    }

}
