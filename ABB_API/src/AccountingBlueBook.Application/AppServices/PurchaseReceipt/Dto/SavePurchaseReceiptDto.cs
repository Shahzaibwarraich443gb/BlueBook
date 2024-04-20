using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.ReceivedPayment.Dto;
using AccountingBlueBook.AppServices.Recurringinvoice.dto;
using AccountingBlueBook.Entities.NormalizeEntities;
using System;
using System.Collections.Generic;

namespace AccountingBlueBook.AppServices.PurchaseReceipt.Dto
{
    public class SavePurchaseReceiptDto : FullAuditedEntityDto<long>
    {
        public SavePurchaseReceiptDto()
        {
            Emails = new List<string>();
        }
        public long? Id { get; set; }
        public long? InvoiceID { get; set; } 
        public string InvoiceNo { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? PurchaseReceiptDate { get; set; }
        public int? RefPaymentMethodID { get; set; }
        public int? RefDepositToAccountId { get; set; }
        public int? RefInvoiceType { get; set; }
        public int? RefInvoiceStatus { get; set; }
        public long? RefCashEquivalentsAccountId { get; set; }
        public long? RefSupplierId { get; set; }
        public int? RefVendorId { get; set; }
        public string ReferenceNo { get; set; }
        public List<string> Emails { get; set; }
        public decimal? Total { get; set; }
        public decimal? PaidAmount { get; set; }
        public long? RefCustomerID { get; set; }
        public long? RefCompanyID { get; set; }
        public int? RefPaymentTypeId { get; set; }

        public long? CheckNo { get; set; }

        public string Note { get; set; }
        //public virtual List<ReceviedPayment> ReceivedPayments { get; set; }
        public virtual ICollection<RecurringInvoiceDetailDto> puchaseReceiptDto { get; set; }

        public virtual ChargeCardDto ChargeCard { get; set; }
    }
    

    //public class ChargeCardDto : FullAuditedEntityDto<long>
    //{
    //    public int? Duration { get; set; }
    //    public decimal? Amount { get; set; }
    //    public string CardType { get; set; }
    //    public string CardHolderName { get; set; }
    //    public string CardNumber { get; set; }
    //    public string ExpDate { get; set; }
    //    public string CCVNo { get; set; }
    //    public string CustomerEmail { get; set; }
    //}

}
