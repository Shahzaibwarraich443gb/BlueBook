using AccountingBlueBook.AppServices.Recurringinvoice;
using Microsoft.AspNetCore.Authorization;
using Quartz;
using System.Threading.Tasks;

namespace AccountingBlueBook.Web.Host.Jobs
{
    [DisallowConcurrentExecution]
    public class RecurringInvoiceJob : IJob
    {
        public static string jobInterval = "0 0 8 * * ?"; //"0/10 * * * * ?";   

        private readonly IRecurringInvoiceAppService recurringInvoiceAppService;

        public RecurringInvoiceJob(IRecurringInvoiceAppService recurringInvoiceAppService)
        {
            this.recurringInvoiceAppService = recurringInvoiceAppService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await recurringInvoiceAppService.RecurringInvoiceJob();
        }
    }
}
