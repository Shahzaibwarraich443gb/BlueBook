using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.JournalVoucher.Dto
{
    public class VoucherList : FullAuditedEntity<long>
    {
        public DateTime? Date { get; set; }
        public string VoucherNo { get; set; }
        public string VoucherTypeCode { get; set; }
        public long? InvoiceId { get; set; }
        public string Company { get; set; }
        public string SrNo { get; set; }
        public decimal? Cr_Amount { get; set; }
        public decimal? Dr_Amount { get; set; }
        public string AccountName { get; set; }
        public object LastModificationTime { get; set; }
        public string AddedBy { get; set; }
        public string LastModifierUserId { get; set; }
        public object CreationTime { get; set; }
    }
}
