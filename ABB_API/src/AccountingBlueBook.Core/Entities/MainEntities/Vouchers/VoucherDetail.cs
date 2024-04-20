using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingBlueBook.Entities.MainEntities.Vouchers
{
    [Table("VoucherDetail")]
    public class VoucherDetail : FullAuditedEntity<long>, IMustHaveTenant

    {
      
        public int TenantId { get; set; }
        public long? RefCompanyId { get; set; }
        public int? SrNo { get; set; }
        public long? RefChartOfAccountId { get; set; }
        public long? RefCustomerId { get; set; }       // todo: remove this column
        public string Note { get; set; }
        public decimal? Dr_Amount { get; set; }
        public decimal? Cr_Amount { get; set; }
        public long? PartnerId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public long? InvoiceId { get; set; }
        public string AccountName { get; set; }
        public long? BankId { get; set; }

        public long VoucherId { get; set; }

        [ForeignKey("VoucherId")]
        public Voucher VoucherFk { get; set; }

    }
}
