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
    [Table("VendorAttachments")]
    public  class VendorAttachment : FullAuditedEntity
    {
        public string FileName { get; set; }
        public int TenantId { get; set; }
        public bool IsVendor { get; set; }
        public bool IsEmail { get; set; }
        public int VendorId { get; set; }
        [ForeignKey("VendorId")]
        public Vendor Vendor { get; set; }

    }
}
