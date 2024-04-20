using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    public class LedgerHeaders : Entity<long>
    {
        public long CustomerId { get; set; }
        public long CompanyId { get; set; }
        public string Headers { get; set; }
        public string LedgerType { get; set; }
    }
}
