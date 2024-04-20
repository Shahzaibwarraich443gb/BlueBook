using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Invoices.dto
{
    public class InvoiceDetailDto : EntityDto<long>
    {
        public long? InvoiceDetailId { get; set; }
        public long? RefPaidInvoiceID { get; set; }
        public long? InvoiceID { get; set; }
        public long? RefProducID { get; set; }
        public long? RefChartOfAccountID { get; set; }
        public string Description { get; set; }
        public long? Quantity { get; set; }
        public long? Rate { get; set; }
        public long? Discount { get; set; }
        public long? SaleTax { get; set; }
        public long? Amount { get; set; }
        public long? PaidAmount { get; set; }
        public bool? IsPaid { get; set; }
        public long? RefCustomerID { get; set; }
        public long? InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public decimal? DiscountAmount { get; set; }
    }
}
