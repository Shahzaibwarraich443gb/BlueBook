using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Invoices.dto;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Recurringinvoice
{
    public interface IRecurringInvoiceAppService : IApplicationService
    {
        Task<InvoiceDto> Get(EntityDto<long> input);
        Task RecurringInvoiceJob();
    }
}
