using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingBlueBook.Entities.MainEntities.Estimate
{
    [Table("EstimateDetails")]
    public class EstimateDetail : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long? RefPaidInvoiceId { get; set; }
        public long? RefProducId { get; set; }
        public long? RefChartOfAccountId { get; set; }
        public string Description { get; set; }
        public long? Quantity { get; set; }
        public long? Rate { get; set; }
        public long? Discount { get; set; }
        public long? SaleTax { get; set; }
        public long? Amount { get; set; }
        public decimal? PaidAmount { get; set; }
        public bool? IsPaid { get; set; }
        public long? RefCustomerId { get; set; }

        public long InvoiceId { get; set; }

        //[ForeignKey("InvoiceId")]
        //public Invoice InvoiceFk { get; set; }

    }
}
