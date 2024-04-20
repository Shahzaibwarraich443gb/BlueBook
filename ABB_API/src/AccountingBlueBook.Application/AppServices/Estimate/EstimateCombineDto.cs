using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using AccountingBlueBook.Entities.MainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Estimate
{
    public class EstimateCombineDto
    {

        public Invoice? Invoice { get; set; }
        public List<InvoiceDetail> InvoiceDetail { get; set; } //
        public string Company { get; set; }
        public Customer? Customer { get; set; }
        public decimal? Balance { get; set; }
        public List<PaymentTermList> PaymentTermList { get; set; }
        public List<ProductService> ProductServices { get; set; }


    }
}
