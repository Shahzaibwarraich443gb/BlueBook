using Abp.Domain.Entities;
using AccountingBlueBook.Entities.Main;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingBlueBook.Entities.NormalizeEntities
{
    [Table("SpouseAddress")]
    public class SpouseAddress : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; } 
        public long SpouseId { get; set; }
        [ForeignKey("SpouseId")]
        public Spouse Spouse { get; set; }

        public int? PhoneId { get; set; }
        [ForeignKey("PhoneId")]
        public Phone Phone { get; set; }
    }
}
