using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Recurring_Invoice.dto
{
    public class CompanyCardDto: Entity<long>
    {

        public long CardID { get; set; }
        public string Card_Type { get; set; }
        public string Card_Holder_Name { get; set; }
        public string Card_Number { get; set; }
        public string Exp_Date { get; set; }
        public string CCV_No { get; set; }
        public long ref_CustomerId { get; set; }
      //  public bool IsActive { get; set; }
        public long AddedByID { get; set; }
        public DateTime AddedDate { get; set; }
        public long ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsCustomer { get; set; }


    }
}
