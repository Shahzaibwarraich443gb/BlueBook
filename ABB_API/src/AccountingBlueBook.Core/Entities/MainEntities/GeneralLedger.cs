using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.MainEntities.Customers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingBlueBook.Entities.MainEntities
{
    public class GeneralLedger: FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int CompanyId { get; set; }
        public int? CustomerId { get; set; }
        public long VendorId { get; set; }  
        public int InvoiceType { get; set; }
        public string VoucherNo { get; set; }
        public long CreatedBy { get; set; }
        public long InvoiceId { get; set; }
        public string Title { get; set; }
        public double Balance { get; set; }
        public string CreatorUserName { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        public virtual ICollection<GeneralLedgerDetails> GeneralLedgerDetails { get; set; }
    }

    public class GeneralLedgerDetails: Entity<long>
    {
        public long ChartOfAccountId { get; set; }
        public double DebitAmount { get; set; }
        public double CreditAmount { get; set; }
        public long GeneralLedgerId { get; set; }

    }
}
