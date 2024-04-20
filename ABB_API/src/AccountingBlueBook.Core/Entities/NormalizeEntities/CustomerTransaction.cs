using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities
{
    public class CustomerTransaction
    {
        public DateTime? invoiceDate { get; set; }
        public string? type { get; set; }
        public string? invoiceCode { get; set; }
        public string? product { get; set; }
        public string? description { get; set; }
        public string? csr { get; set; }
        public decimal? balance { get; set; }
        public decimal? total { get; set; }
        public string? status { get; set; }
    }
}
