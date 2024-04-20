using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    public  class Vendor : FullAuditedEntity
    {
        public string BusinessName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public long TaxId { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string SSN { get; set; }
        public string VendorName { get; set; }

        public int? VenderTypeId { get; set; }

        [ForeignKey("VenderTypeId")]
        public VenderType VenderType  { get; set; }
        public int CompanyId { get; set; }
 
    }
}
