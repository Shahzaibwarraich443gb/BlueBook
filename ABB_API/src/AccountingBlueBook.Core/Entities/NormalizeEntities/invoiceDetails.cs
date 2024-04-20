using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.NormalizeEntities
{
    public class invoiceDetails
    {
        public long? InvoiceId { get; set; }
        public long? InvoiceDetailId { get; set; }
        public int? ProductId { get; set; }
        public long? Quantity { get; set; }

        public long? Amount { get; set; }
        public bool? IsPaid { get; set; }
        public long? Rate { get; set; }
        public long? SaleTax { get; set; }
        public long? Discount { get; set; }
        public int? CustomerId { get; set; }
        public string InvoiceNo { get; set; }
        public string Note { get; set; }
        public bool? IsSendLater { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public DateTime? CreationTime { get; set; }
        public string Email { get; set; }
        public string CustomerEmail { get; set; }
        public int? RefTermId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string Product { get; set; }
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public long? RefChartOfAccountId { get; set; }
        public long? RefCustomerId { get; set; }
        public long? PaidAmount { get; set; }
        public long? VendorId { get; set; }
        public DateTime? CreditNoteDate { get; set; }
        public string ComAddress { get; set; }
        public string ComCity { get; set; }
        public string ComState { get; set; }
        public string ComPostCode { get; set; }
        public string ComCountry { get; set; }
        public string ComEmail { get; set; }
        public string ComPhone { get; set; }
        public string CompanyName { get; set; }
        public string CSR { get; set; }
    }
}
