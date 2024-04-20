using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.NormalizeEntities
{
    [Table("CustomerPhoneNumber")]
    public class CustomerPhoneNumber : FullAuditedEntity
    {
        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public int? PhoneId { get; set; }
        [ForeignKey("PhoneId")]
        public Phone Phone { get; set; }
    }
}
