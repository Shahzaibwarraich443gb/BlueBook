using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingBlueBook.Entities.MainEntities
{
    [Table("AccountTypes")]
    public class AccountType : FullAuditedEntity
    {
        public string Name { get; set; }
        public long Code { get; set; }
        public string Description { get; set; }
        public AccountNature AccountNature { get; set; }
    }
}
