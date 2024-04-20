using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    public class Ledger: FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int CompanyId { get; set; }
        public long CustomerId { get; set; }
        public long VendorId { get; set; }  
        public long InvoiceType { get; set; }
        public long VoucherId { get; set; }
        public long InvoiceId { get; set; }
        public long ChartOfAccountId { get; set; }
        public double DebitAmount { get; set; }
        public double CreditAmount { get; set; }
        public double Balance { get; set; }
    }
}
