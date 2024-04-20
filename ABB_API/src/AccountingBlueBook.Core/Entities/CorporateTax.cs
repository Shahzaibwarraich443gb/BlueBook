using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.MainEntities.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities
{
    public class CorporateTax: FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int CustomerId { get; set; } 
        public int FinancialYear { get; set; }
        public string MonthlyData { get; set; }
        public string OtherIncome { get; set; }
        public string CostOfSale { get; set; }
        public string OtherExpense { get; set; }
        public int? LegalStatus { get; set;}
        public int Tenure { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
