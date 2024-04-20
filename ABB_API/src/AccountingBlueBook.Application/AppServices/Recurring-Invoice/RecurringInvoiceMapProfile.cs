using AccountingBlueBook.AppServices.Recurringinvoice.dto;
using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;

namespace AccountingBlueBook.AppServices.Recurring_Invoice
{
    public class RecurringInvoiceMapProfile: Profile
    {
        public RecurringInvoiceMapProfile()
        {
            CreateMap<RecurringInvoiceDto, RecurringInvoice>();
        }
    }
}
