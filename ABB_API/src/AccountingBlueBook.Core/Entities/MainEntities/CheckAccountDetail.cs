using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    public class CheckAccountDetail: FullAuditedEntity<long>
    {
        public long AccountId { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }  
        public long CustomerId { get; set; }
        public long CheckId { get; set; }
    }
}
