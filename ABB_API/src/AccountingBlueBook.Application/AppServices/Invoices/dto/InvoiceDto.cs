using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Invoices.dto
{
    public class InvoiceDto : FullAuditedEntityDto<long>
    {
        public long? InvoiceId  {get; set; }
        public string InvoiceNo { get; set; }
        public long? RefCompanyID { get; set; }
        public int? RefInvoiceType { get; set; }
        public long? RefCustomerID { get; set; }
        public long? RefSupplierID { get; set; }
        public int? RefTermID { get; set; }
        public int? RefPaymentMethodID { get; set; }
        public long? RefDepositToAccountID { get; set; }
        public long? RefCashEquivalentsAccountID { get; set; } 
        public List<string> Email { get; set; } = new List<string>();
        public bool? IsSendLater { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public DateTime? EstimateDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? CreditNoteDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? PurchaseReceiptDate { get; set; }
        public DateTime? SaleReceiptDate { get; set; }
        public string RefrenceNo { get; set; }
        public decimal? AmountReceived { get; set; }
        public decimal? Total { get; set; }
        public string Note { get; set; }
        public decimal? PaidAmount { get; set; }
        public int? RefInvoiceStatusID { get; set; }
        public bool? IsPaid { get; set; }
        public bool IsActive { get; set; }
        public bool? IsRecurring { get; set; }
        public int? FrequencyId { get; set; }
        public string Frequency { get; set; }
        public int? DurationId { get; set; }
        public string Duration { get; set; }
        public int? InvoiceGroupID { get; set; }
        public DateTime? DepositDate { get; set; }
        public DateTime? ClearanceDate { get; set; }
        public bool? CheckPrinted { get; set; }
        public long? RefEmployeeID { get; set; }
        public int? RefPaymentTypeID { get; set; }
        public int? RefCardID { get; set; }
        public DateTime? RecurringInvoiceNextCreationDate { get; set; }

        public virtual ICollection<InvoiceDetailDto> InvoiceDetails { get; set; }
        public int? RefInvoiceStatus { get; internal set; }
    }
}
