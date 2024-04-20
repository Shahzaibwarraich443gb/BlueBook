using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.CorporateTaxes
{
    public class CorporateTaxDto: FullAuditedEntityDto<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int CustomerId { get; set; }
        public int FinancialYear { get; set; }
        public string monthlyData { get; set; }
        public int Tenure { get; set; }
        public string OtherIncome { get; set; }
        public string CostOfSale { get; set; }
        public string OtherExpense { get; set; }
        public int? LegalStatus { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
