using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    public class Check: FullAuditedEntity<long>, ISoftDelete
    {
        public string CheckCode { get; set; }
        public string PayeeId { get; set; }
        public double TotalAmount { get; set; }
        public long BankId { get; set; }
        public string Notes { get; set; }
        public long CompanyId { get; set; }
        public bool IsDeleted { get; set; }
        public List<CheckProductDetail> CheckProductDetails { get; set; }
        public List<CheckAccountDetail> CheckAccountDetails { get; set; }

    }
}
