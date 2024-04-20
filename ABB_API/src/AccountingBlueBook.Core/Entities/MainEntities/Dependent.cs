using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.MainEntities.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    public class Dependent  :FullAuditedEntity
    {
        public string Name { get; set; }
        public string SSN { get; set; }
        public string relation { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer customer { get; set; }

    }
}
