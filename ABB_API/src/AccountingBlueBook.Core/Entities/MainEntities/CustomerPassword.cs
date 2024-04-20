using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.MainEntities.Customers;
using Castle.Components.DictionaryAdapter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    public class CustomerPassword:FullAuditedEntity
    {
        public string Type { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

        public string? Description { get; set; }
        public string? url { get; set; }

        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }


    }
}
