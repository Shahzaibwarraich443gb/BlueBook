using AccountingBlueBook.Entities.MainEntities.Invoices;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.CreditNote.Dto
{
    public class CreditNoteMap : Profile
    {
        public CreditNoteMap()
        {
            CreateMap<InvoiceDetail, CreditNoteDto>().ReverseMap();
            CreateMap<Invoice, CreditNoteDto>().ReverseMap();
        }
    }

}
