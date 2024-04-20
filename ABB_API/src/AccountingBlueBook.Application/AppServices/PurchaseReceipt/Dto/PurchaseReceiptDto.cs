using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.PurchaseReceipt.Dto
{
    public class PurchaseReceiptDto : FullAuditedEntityDto
    {
        public long? InvoiceID { get; set; }
        public long? SplitInvoiceID { get; set; }
        public string Email { get; set; }
        public long? RefCompanyId { get; set; }
        public string InvoiceNo { get; set; }
        public bool? IsCheck { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public decimal? AmountReceived { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int? ref_PaymentMethodID { get; set; }
        public string RefrenceNo { get; set; }
        public decimal? PaidAmount { get; set; }
        public long? ref_CustomerID { get; set; }
        public long? ref_SupplierID { get; set; }
        public long? ref_PaidInvoiceID { get; set; }
        public long? ref_DepositToAccountID { get; set; }
        public string Note { get; set; }
        public string amountinwords { get; set; }
        public string Product { get; set; }
        public string Description { get; set; }
        public List<InvoiceDetail> InvoiceDetails { get; set; }

    }
}
