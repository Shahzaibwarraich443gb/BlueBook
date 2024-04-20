using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities.LinkedAccounts
{
    [Table("LinkedAccounts")]
    public class LinkedAccount : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long CompanyID { get; set; }
        public string NameAccountToLink { get; set; }
        public long? RefChartOfAccountId { get; set; }
        public bool? ReadOnly { get; set; }
    }
}
