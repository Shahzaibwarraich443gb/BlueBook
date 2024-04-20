using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingBlueBook.Entities.MainEntities.Estimate
{
    [Table("Estimates")]
    public class Estimates : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public string InvoiceNo { get; set; }
        public long RefCompanyId { get; set; }
        public EstimateEnum? Estimatetype { get; set; }
        public long? RefCustomerId { get; set; }
        public long? RefSupplierId { get; set; }
        public int? RefTermId { get; set; }
        public int? RefPaymentMethodId { get; set; }
        public long? RefDepositToAccountId { get; set; }
        public long? RefCashEquivalentsAccountId { get; set; }
        public string Email { get; set; }
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
        public InvoiceStatus? InvoiceStatus { get; set; }
        public bool? IsPaid { get; set; }
        public bool IsActive { get; set; }
        public bool? IsRecurring { get; set; }
        public int? FrequencyId { get; set; }
        public string Frequency { get; set; }
        public int? DurationId { get; set; }
        public string Duration { get; set; }
        public int? InvoiceGroupId { get; set; }
        public DateTime? DepositDate { get; set; }
        public DateTime? ClearanceDate { get; set; }
        public bool? CheckPrinted { get; set; }
        public long? RefEmployeeId { get; set; }
        public int RefPaymentTypeId { get; set; }
        public int RefCardId { get; set; }
        public DateTime? RecurringInvoiceNextCreationDate { get; set; }

        public virtual ICollection<EstimateDetail> EstimateDetails { get; set; }
    }
}
