using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.MainEntities.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    [Table("CustomerUsers")]
    public class CustomerUser : FullAuditedEntity
    {
        public string Type { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }
}
