using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.NormalizeEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.PurchaseInvoice.Dto
{
    public class SavePurchaseInvoice : FullAuditedEntityDto<long>
    {
        public long? Id { get; set; }
        public long? InvoiceId { get; set; }
        public long? VendorId { get; set; }
        public string? RefNo { get; set; }
        public int? RefTermID { get; set; }
        public string Note { get; set; }
        public long? Total { get; set; }
        public DateTime? PurchaseInvoiceDate { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public virtual List<PurchaseInvoiceDto> PurchaseInvoice { get; set; }
        public virtual List<PurchaseInvoiceAccountDto> PurchaseInvoiceAccount { get; set; }
        public string InvoiceNo { get; set; }
    }
    public class PurchaseInvoiceDto
    {
        public long InvoiceDetailID { get; set; }
        public long? RefInvoiceID { get; set; }
        public long? RefPaidInvoiceID { get; set; }
        public long? RefProducID { get; set; }
        public long? RefChartOfAccountID { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Discount { get; set; }
        public decimal? SaleTax { get; set; }
        public decimal? Amount { get; set; }
        public decimal? PaidAmount { get; set; }
        public bool? IsPaid { get; set; }
        public long? RefCustomerID { get; set; }
    }
    public class PurchaseInvoiceAccountDto
    {
        public long InvoiceDetailID { get; set; }
        public long? RefInvoiceID { get; set; }
        public long? RefPaidInvoiceID { get; set; }
        public long? RefChartOfAccountID { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public decimal? PaidAmount { get; set; }
        public bool? IsPaid { get; set; }
        public long? RefCustomerID { get; set; }
    }
}
