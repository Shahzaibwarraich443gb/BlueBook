using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingBlueBook.Entities.MainEntities
{
    [Table("Transactions")]
    public class Transaction : FullAuditedEntity
    {
        public DateTime? TransactionDate { get; set; }
        public string TransactionNo { get; set; }
        public long? RefCompanyID { get; set; }
        public long? RefCustomerID { get; set; }
        public string TranDescription { get; set; }
        public string ReferalNo { get; set; }
        public DateTime? DebitDate { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public DateTime? DepositDate { get; set; }
        public long? ChartOfAccountID { get; set; }
        public decimal? CreditAmount { get; set; }
        public string TransactionType { get; set; }
        public int? BankId { get; set; }
        public string Status { get; set; }
        public decimal? DebitAmount { get; set; }
        public decimal? Balance { get; set; }
        public decimal? VerifiedBy { get; set; }
        public int? PaymentReceiveID { get; set; }
        public string PaymentReceiveNo { get; set; }
        public bool? ImportFlag { get; set; }
        public int? ImportID { get; set; }
        public int? InvoiceTypeID { get; set; }
        public bool? IsDeleted { get; set; }
        public string NoteStatus { get; set; }
        public bool? FlagedInd { get; set; }
        public bool? VerifiedInd { get; set; }
        public bool? RefReconciliationId { get; set; }
        public bool? SplitReconInd { get; set; }
        public bool? BatchReconInd { get; set; }
    }
}
