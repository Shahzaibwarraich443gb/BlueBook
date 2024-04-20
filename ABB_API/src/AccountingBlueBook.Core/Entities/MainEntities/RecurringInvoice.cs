using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    public class RecurringInvoice: FullAuditedEntity<long>, IMustHaveTenant
    {
        public long CustomerId { get; set; }

        public int DurationId { get; set; }

        public int FrequencyId { get; set; }

       //Duration Data
        public DateTime? DurationDateTill { get; set; }
        public int? DurationAmount { get; set; }
        public int? ExecutedAmount { get; set; }


        //Frequency Data
        public DateTime? FrequencyCustomDate { get; set; }
        public int? FrequencyEveryDayCount { get; set; }
        public int? FrequencyWeek { get; set; }
        public int? FrequencyMonth { get; set; }
        public DateTime? FrequencyAnnualDate { get; set; }


        public string InvoiceData { get; set; }
        public int TenantId { get; set; }
        public DateTime LastExecution { get; set; }
        public bool SendMail { get; set; }
        public int CustomerCardId { get; set; }

    }
}
