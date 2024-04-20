using Abp.Application.Services.Dto;
using System;

namespace AccountingBlueBook.Entities.MainEntities.Customers.Dto
{
    public class CustomerTransactionDto:EntityDto
    {
        public long? InvoiceId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string Type { get; set; }
        public string InvoiceCode { get; set; }
        public string Product { get; set; }
        public string Description { get; set; }
        public string Csr { get; set; }
        public decimal? Balance { get; set; }
        public decimal? Total { get; set; }
        public long? RefCustomerId { get; set; }
        public string Status { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public long? LastModifierUserId { get; set; }
        public string AddedBy { get; set; }
        public string Company { get; set; }
        public string OrignalInvoiceNo { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public string RefrenceNo { get; set; }
        public string ComAddress { get; set; }
        public string ComCity { get; set; }
        public string ComState { get; set; }
        public string ComPostCode { get; set; }
        public string ComCountry { get; set; }
        public string ComEmail { get; set; }
        public string ComPhone { get; set; }
    }
}
