using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities.Reports.Dto
{
    public class DailyReceiptDto : EntityDto
    {
        public long? InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public long? RefCustomerId { get; set; }
        public string Company { get; set; }
        public string PaymentMethod { get; set; }
        public long? CompanyId { get; set; }
        public long? RefDepositToAccountId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string EmployeeName { get; set; }
        public int? RefPaymentMethodId { get; set; }
        public decimal? Total { get; set; }
        public string CustomerName { get; set; }
        public string CSR { get; set; }
        public string AccountDescription { get; set; }
        public decimal? OpenBalance { get; set; }
        public decimal? PaidAmount { get; set; }
    }
}
