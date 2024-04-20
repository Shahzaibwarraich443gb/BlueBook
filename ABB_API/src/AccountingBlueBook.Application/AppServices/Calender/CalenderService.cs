using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using AccountingBlueBook.AppServices.Invoices;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Calender
{
    public class CalenderService : AccountingBlueBookAppServiceBase, ICalenderService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IRepository<InvoiceDetail, long> _invoiceDetailRepository;
        private readonly IRepository<CalendarEvent, long> _calendarEventRepository;
        public CalenderService(IRepository<InvoiceDetail, long> invoiceDetailRepository,
                                 IInvoiceRepository invoiceRepository, IRepository<CalendarEvent, long> calendarEventRepository)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceDetailRepository = invoiceDetailRepository;
            _calendarEventRepository = calendarEventRepository;
        }
        public async Task<string> AddInvoiceReminder(long id)
        {
            var invoice = await _invoiceRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            var calendar = await _calendarEventRepository.GetAll().FirstOrDefaultAsync(x => x.Subject == invoice.InvoiceNo && x.IsDeleted!=true);
            if (invoice != null)
            {
                if (calendar != null)
                {
                   // throw new UserFriendlyException("Invoice already exists in the system reminders");
                    return "error";
                }
                else
                {
                    CalendarEvent e = new CalendarEvent()
                    {
                        Start = invoice.InvoiceDate,
                        End = invoice.InvoiceDueDate,
                        Subject = $"{invoice.InvoiceNo}",
                        Description = $"Your invoice no: {invoice.InvoiceNo} will be due on {invoice.InvoiceDueDate.Value.ToShortDateString()}",
                        Priority = 1,
                        ThemeColor = GetThemeColor(1),
                        refUserId = AbpSession.UserId,
                        refCompanyId = invoice.RefCompanyId,
                        CreatedBy = AbpSession.TenantId,
                        CreationTime = DateTime.Now,
                        refModuleId = "Invoice",
                        refReference = invoice.InvoiceNo
                    };
                    await _calendarEventRepository.InsertAsync(e);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    return "success";
                }
            }
            else
            {
                return "invoiceNo not found";
            }
        }
        private string GetThemeColor(int priority)
        {
            try
            {
                switch (priority)
                {
                    case 1:
                        return "darkred";
                    case 2:
                        return "darkorange";
                    case 3:
                        return "cornflowerblue";
                    default:
                        return "";
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller: Calendar, Action: GetThemeColor", ex);
                return "";
            }
        }
    }
}
