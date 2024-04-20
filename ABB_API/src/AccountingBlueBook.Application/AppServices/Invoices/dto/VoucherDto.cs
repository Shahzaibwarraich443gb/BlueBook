using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Invoices.dto
{
    public class VoucherDto
    {
        public long? CompanyID { get; set; }
        public long? LocationID { get; set; }
        public string VoucherNo { get; set; }
        public string VoucherTypeCode { get; set; }
        public long? TransactionTypeID { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public long? PaymentType { get; set; }
        public long? PaymentMode { get; set; }
        public long? ChequeNo { get; set; }
        public long? InvoiceID { get; set; }
        public int TenantId { get; set; }
    }
}
