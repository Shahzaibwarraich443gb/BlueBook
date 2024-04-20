using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities.Customers;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingBlueBook.Entities.NormalizeEntities
{
    [Table("CustomerAddress")]
    public class CustomerAddress : FullAuditedEntity
    {   
        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public int? AddressId { get; set; }
        [ForeignKey("AddressId")]
        public Address Address { get; set; }
    }
}
