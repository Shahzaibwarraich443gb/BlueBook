using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.Main
{
    [Table("VendorAddresses")]
    public class VendorAddress : FullAuditedEntity
    {
        public string CompleteAddress { get; set; }
        [StringLength(maximumLength: 240)]
        public string City { get; set; }
        [StringLength(maximumLength: 240)]
        public string State { get; set; }
        [StringLength(maximumLength: 240)]
        public string Country { get; set; }
        [StringLength(maximumLength: 240)]
        public string PostCode { get; set; }
        [StringLength(maximumLength: 240)]
        public string Fax { get; set; }
        [StringLength(maximumLength: 240)]
        public string Type { get; set; }
        public bool IsPrimary { get; set; }
        public int? VendorId { get; set; }
        [ForeignKey("VendorId")]
        public Vendor Vendor { get; set; }
    }
}
