using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.Main
{
    [Table("SourceReferralTypes")]
    public class SourceReferralType : FullAuditedEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        
        public int EmailId { get; set; }
        [ForeignKey("EmailId")]
        public Email Email { get; set; }

        public int PhoneId { get; set; }
        [ForeignKey("PhoneId")]
        public Phone Phone { get; set; }

        public int AddressId { get; set; }
        [ForeignKey("AddressId")]
        public Address Address { get; set; }

        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
    }
}
