using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    public class CheckProductDetail: FullAuditedEntity<long>
    {
        public long ProductId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public double Rate { get; set; }
        public double SaleTax { get; set; }
        public double Amount { get; set; }
        public int CustomerId { get; set; }
        public long CheckId { get; set; }
    }
}
