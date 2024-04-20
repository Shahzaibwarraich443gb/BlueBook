using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Invoices.dto
{
    public class PrintDto : FullAuditedEntityDto<long>
    {
        public DateTime? PaymentDate { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string OrignalInvoiceNo { get; set; }
        public string RefrenceNo { get; set; }
        public string Note { get; set; }
        public string CSR { get; set; }
        public string OpenBalance { get; set; }
        // Company
        public string CompanyName { get; set; }
        public string ComAddress { get; set; }
        public string ComCity { get; set; }
        public string ComState { get; set; }
        public string ComPostCode { get; set; }
        public string ComCountry { get; set; }
        public string ComEmail { get; set; }
        public string ComPhone { get; set; }
        // Customer
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public string CustomerBussinessName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerState { get; set; }
        public string CustomerPostCode { get; set; }
        public string CustomerCountry { get; set; }
        public virtual List<PrintDetail> PrintDetails { get; set; }
    }
    public class PrintDetail
    {
        public string CSR;
        public int? Amount { get; set; }
        public string Product { get; set; }
        // Invoice
        public long? InvoiceId { get; set; }
        public long? InvoiceDetailId { get; set; }
        public int? ProductId { get; set; }
        public long? Quantity { get; set; }
        public bool? IsPaid { get; set; }
        public long? Rate { get; set; }
        public long? SaleTax { get; set; }
        public long? Discount { get; set; }
        // Received Payment
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public decimal? OrigionalAmount { get; set; }
        public decimal? OpenBalance { get; set; }
        public int? RefPaymentMethodID { get; set; }
        public string Description { get; set; }
    }
}
