using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.Main
{
    [Table("SalesPersonTypes")]
    public class SalesPersonType : FullAuditedEntity
    {
        public string Name { get; set; }

        public string Code { get; set; }
        public bool IsActive { get; set; }
        public string CompleteAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }
        public string Fax { get; set; }
        public string Type { get; set; }
        public bool IsPrimary { get; set; }
        public int PhoneId { get; set; }
        [ForeignKey("PhoneId")]
        public Phone Phone { get; set; }

        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
    }
}
