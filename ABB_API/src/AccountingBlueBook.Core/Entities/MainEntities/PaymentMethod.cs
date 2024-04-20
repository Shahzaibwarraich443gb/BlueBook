using Abp.Domain.Entities;
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
    [Table("PaymentMethod")]
    public class PaymentMethod : FullAuditedEntity, IMustHaveTenant
    {
        public int TenantId { get; set; } 
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
