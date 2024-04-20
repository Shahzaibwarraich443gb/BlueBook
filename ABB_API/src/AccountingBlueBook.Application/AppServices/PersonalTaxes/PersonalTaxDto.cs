using AccountingBlueBook.Entities;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.PersonalTaxes
{
    public class PersonalTaxDto
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

        public int SpouseId { get; set; }

        public virtual Spouse Spouse { get; set; }

        public List<int> DependentIds { get; set; }

        public virtual List<Dependent> Dependents { get; set; }


        public ICollection<long> IncomeDetailIds { get; set; }

        public virtual ICollection<IncomeDetails> incomeDetails { get; set; }

        public int CustomerId { get; set; }

        public string? OtherExpense { get; set; }

    }
}
