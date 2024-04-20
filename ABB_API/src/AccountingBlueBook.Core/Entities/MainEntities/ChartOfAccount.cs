using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingBlueBook.Entities.Main
{
    [Table("ChartOfAccounts")]
    public class ChartOfAccount : FullAuditedEntity
    {
        public long ChartOfAccountId { get; set; }
        public AccountNature AccountNature { get; set; } = AccountNature.Asset;
        public string AccountDescription { get; set; }
        public long AccountCode { get; set; }
        public bool AccountStatus { get; set; }
        public string Detail { get; set; }
        public bool AllowDepreciation { get; set; }
        public decimal CreditLimitAmount { get; set; }
        public bool IsCashFlow { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefaultAccount { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        public int? AccountTypeId { get; set; }
        [ForeignKey("AccountTypeId")]
        public AccountType AccountType { get; set; }
        public int? MainHeadId { get; set; }
        [ForeignKey("MainHeadId")]
        public MainHead MainHead { get; set; }

        public decimal Balance { get; set; }
    }
}
