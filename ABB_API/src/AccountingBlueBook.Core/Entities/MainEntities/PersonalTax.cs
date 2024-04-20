using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingBlueBook.Entities.MainEntities
{
    public class PersonalTax : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public int FinancialYear { get; set; }

        public int TaxFillingStatus { get; set; }

        public int Form { get; set; }

        public int Tenure { get; set; }

        public string FilerOccupation { get; set; }

        public string BankName { get; set; }

        public string RoutingNumber { get; set; }

        public string AccountNumber { get; set; }

        public string FilersLicenseNumber { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int IssueState { get; set; }

        public int ThreeDigitCode { get; set; }

        public string? OtherExpense { get; set; }
        //public long SpouseId { get; set; }

        //[ForeignKey("SpouseId")]
        //public virtual Spouse Spouse { get; set; }

        //public string DependentIds { get; set; }

        //public virtual ICollection<Dependent> Dependents { get; set; }

        public string IncomeDetailIds { get; set; }

        public virtual ICollection<IncomeDetails> IncomeDetails { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }
}
