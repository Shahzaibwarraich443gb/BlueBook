using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities
{
    public class IncomeDetails : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string IncomeDescription { get; set; }
        public double Amount { get; set; }
        public int FederalWH { get; set; }
        public int StateWH { get; set; }

        public long PersonalTaxId { get; set; } 
    }
}
