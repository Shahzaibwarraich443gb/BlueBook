using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Calender
{
    public interface ICalenderService : IApplicationService
    {
        Task<string> AddInvoiceReminder(long id);
    }
}
