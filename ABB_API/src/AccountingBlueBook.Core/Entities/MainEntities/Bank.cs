using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccountingBlueBook.Entities.MainEntities
{
    [Table("Banks")]
    public  class Bank : FullAuditedEntity
    {
        public string TitleofAccount { get; set; }
        public string BankName { get; set; }
        public int AccountNumber { get; set; }
        public int Routing { get; set; }
        public decimal OpenBalance   { get; set; }
        public int StartingCheque { get; set; }
        public int SwiftCode { get; set; }
        public bool IsActive { get; set; }
        public int? BankAddressId { get; set; }

        public long CoaMainHeadId { get; set; }

        [ForeignKey("BankAddressId")]
        public BankAddress  BankAddress { get; set; }
    }
}
