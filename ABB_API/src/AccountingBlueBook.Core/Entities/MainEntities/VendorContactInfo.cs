using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    [Table("VendorContactInfos")]
    public  class VendorContactInfo : FullAuditedEntity
    {
        public string ContactPersonName { get; set; }
        public string ContactTypeName { get; set; }
        public string Fax { get; set; }
        public int? EmailTypeId { get; set; }
        public string EFax { get; set; } 
        public string EmailAddress { get; set; }
        public string WebSite { get; set; }
        public string PhoneNumber { get; set; }
        public bool Primary { get; set; }    
        public int? VendorId { get; set; }
        [ForeignKey("VendorId")]
        public Vendor Vendor { get; set; }

        public int? ContactPersonTypeId { get; set; }
        [ForeignKey("ContactPersonTypeId")]
        public ContactPersonType ContactPersonType { get; set; }
        public int? ContactTypeId { get; set; }

    }
}
