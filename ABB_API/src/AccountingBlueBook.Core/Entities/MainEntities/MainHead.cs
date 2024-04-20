using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.MainEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingBlueBook.Entities.Main
{
    [Table("MainHeads")]
    public class MainHead : FullAuditedEntity
    {
        public string Name { get; set; }
        public long Code { get; set; }
        public int? AccountTypeId { get; set; }
        [ForeignKey("AccountTypeId")]
        public AccountType AccountType { get; set; }
    }
}
