using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.MainEntities.Customers;
using Castle.Core.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    [Table("ContactInfos")]
    public class ContactInfo : FullAuditedEntity
    {
        public string ContactInfoType { get; set; }
        public string Name { get; set; }
        public string NumberType { get; set; }
        public string Number { get; set; }
        public string EmailType { get; set; }
        public string Email { get; set; }
        public bool IsPrimary { get; set; }
        public string Fax { get; set; }
        public string EFax { get; set; }
        public string Website { get; set; } 
        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }
}
