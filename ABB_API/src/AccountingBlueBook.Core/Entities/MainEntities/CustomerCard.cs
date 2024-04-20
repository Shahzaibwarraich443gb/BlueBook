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
    [Table("CustomerCard")]
    public class CustomerCard : FullAuditedEntity
    {
        public long CardID { get; set; }
        public string Card_Type { get; set; }
        public string Card_Holder_Name { get; set; }
        public string Card_Number { get; set; }
        public string Exp_Date { get; set; }
        public string CCV_No { get; set; }
        public long ref_CustomerId { get; set; }
        public bool IsActive { get; set; }
        public long AddedByID { get; set; }
        public DateTime AddedDate { get; set; }
        public long ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsCustomer { get; set; }
    }
}
