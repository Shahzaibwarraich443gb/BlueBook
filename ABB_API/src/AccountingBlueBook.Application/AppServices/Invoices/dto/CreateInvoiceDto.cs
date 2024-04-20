using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Invoices.dto
{
    public class CreateInvoiceDto
    {
        public InvoiceDto Invoice { get; set; }

        public int? TenantId { get; set; }
    }
}
