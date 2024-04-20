using System;

namespace AccountingBlueBook.Entities.NormalizeEntities
{
    public class ReceviedPayment
    {
        public long? InvoiceID { get; set; }
        public long? SplitInvoiceID { get; set; }
        public string InvoiceNo { get; set; }
        public bool? IsCheck { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public decimal? OrigionalAmount { get; set; }
        public decimal? OpenBalance { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int? RefPaymentMethodID { get; set; }
        public string RefrenceNo { get; set; }
        public string Email { get; set; }
        public bool? IsSendLater { get; set; }
        public bool? IsPaid { get; set; }
        public string CustomerEmail { get; set; }
        public decimal? PaidAmount { get; set; }
        public long? RefCustomerID { get; set; }
        public long? RefCompanyID { get; set; }
        public string CompanyName { get; set; }
        public long? RefSupplierID { get; set; }
        public long? RefPaidInvoiceID { get; set; }
        public long? RefDepositToAccountID { get; set; }
        public decimal? Total { get; set; }
        public string Note { get; set; }
        public string Product { get; set; }
        public string ProducIDs { get; set; }
        public string Description { get; set; }
        public long? RefProducID { get; set; }
        public long? InvoiceDetailId { get; set; }
        public long? RefPaidInvoiceId { get; set; }
        public string RP_Invoice { get; set; }
        // for print
        public string ComAddress { get; set; }
        public string ComCity { get; set; }
        public string ComState { get; set; }
        public string ComPostCode { get; set; }
        public string ComCountry { get; set; }
        public string ComEmail { get; set; }
        public string ComPhone { get; set; }
        public string CustomerName { get; set; }
    }
}
