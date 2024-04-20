using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    public class SalesTax : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        [StringLength(100)]
        public string FinancialYear { get; set; }

        public int? LegalStatus { get; set; }

        public int TenureForm { get; set; }

        public double TotalMonthlyAmount { get; set; }

        public double NonTaxableAmount { get; set; }

        public double TaxableSales { get; set; }
        public double SalesTaxAmount { get; set; }

        public double SalesRatePercentage { get; set; }

        public string TaxDataMonthly { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

    }
}
