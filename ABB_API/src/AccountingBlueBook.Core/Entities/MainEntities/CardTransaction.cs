using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    [Table("CardTransactions")]
    public class CardTransaction : FullAuditedEntity
    {                       
        public string PreTransaction { get; set; }
        public string Location { get; set; }
        public string UserEmail { get; set; }
        public string ProTransaction { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
