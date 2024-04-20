using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.NormalizeEntities;
using System;
using System.Collections.Generic;

namespace AccountingBlueBook.AppServices.ReceivedPayment.Dto
{
    public class SaveReceivedPayment : FullAuditedEntityDto<long>
    {
        public SaveReceivedPayment()
        {
            Emails = new List<string>();
        }
        public long? Id { get; set; }
        public long? InvoiceID { get; set; }
        public bool? IsSendLater { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int? RefPaymentMethodID { get; set; }
        public int? RefDepositToAccountId { get; set; }
        public int? RefInvoiceType { get; set; }
        public int? RefInvoiceStatus { get; set; }
        public string ReferenceNo { get; set; }
        public List<string> Emails { get; set; }
        public decimal? Total { get; set; }
        public decimal? PaidAmount { get; set; }
        public long? RefCustomerID { get; set; }
        public long? RefSupplierID { get; set; }
        public long? RefCompanyID { get; set; }
        public int? RefPaymentTypeId { get; set; }
        public string Note { get; set; }
        public virtual List<ReceviedPayment> ReceivedPayments { get; set; }
        public virtual ChargeCardDto ChargeCard { get; set; }
    }
    
    public class ChargeCardDto : FullAuditedEntityDto<long>
    {
        public int? Duration { get; set; }
        public decimal? Amount { get; set; }
        public string CardType { get; set; }
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string ExpDate { get; set; }
        public string CCVNo { get; set; }
        public string CustomerEmail { get; set; }

        public int? TenantId { get; set; }
    }

}
