using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace AccountingBlueBook.AppServices.SalesTaxes
{
    public class SalesTaxDto: EntityDto<long>
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
    }
}
