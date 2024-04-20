using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.JournalVoucher.Dto
{
    public class SaveJournalVouchers : FullAuditedEntityDto<long>
    {
        public DateTime? InvoiceDate { get; set; }
        public string RefInvoiceType { get; set; }
        public string Note { get; set; }
        public virtual List<VoucherDetailDto> VoucherDetails { get; set; }
    }
    public class VoucherDetailDto
    {
        public double DAmount { get; set; }
        public double CAmount { get; set; }
        public string Description { get; set; }
        public long? RefCustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Name { get; set; }
        public int ChartOfAccountID { get; set; }
    }
}
