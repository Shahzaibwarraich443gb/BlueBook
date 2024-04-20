using AccountingBlueBook.AppServices.Invoices.dto;
using AccountingBlueBook.AppServices.Recurringinvoice.dto;
using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Invoices
{
    public class InvoiceMapProfile: Profile
    {
        public InvoiceMapProfile()
        {
            CreateMap<PrintDetail, PrintDetail>();
        }
    }
}
