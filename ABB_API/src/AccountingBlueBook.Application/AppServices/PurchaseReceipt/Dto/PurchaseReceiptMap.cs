using AccountingBlueBook.Entities.MainEntities.Invoices;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.PurchaseReceipt.Dto
{
    public class PurchaseReceiptMap : Profile
    {
        public PurchaseReceiptMap()
        {
            CreateMap<InvoiceDetail, PurchaseReceiptDto>().ReverseMap();
            CreateMap<Invoice, PurchaseReceiptDto>().ReverseMap();
        }
    }

}
