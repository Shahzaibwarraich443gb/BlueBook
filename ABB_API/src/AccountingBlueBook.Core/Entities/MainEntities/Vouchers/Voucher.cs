using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities.Vouchers
{
    [Table("Vouchers")]
    public class Voucher : FullAuditedEntity<long>, IMustHaveTenant
    {
        public Voucher()
        {
            VoucherDetails = new List<VoucherDetail>();
        }

        public long? LocationId { get; set; }
        public string VoucherNo { get; set; }
        public string VoucherTypeCode { get; set; }
        public long? TransactionTypeId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public long? PaymentType { get; set; }
        public long? PaymentMode { get; set; }
        public long? ChequeNo { get; set; }
        public long? InvoiceId { get; set; }
        public int TenantId { get; set; }

        public virtual ICollection<VoucherDetail> VoucherDetails { get; set; }
    }
}
