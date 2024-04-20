using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.Main
{
    [Table("Ethnicities")]

    public class Ethnicity : FullAuditedEntity
    {
        public string Name { get; set; }
        public string Descripition { get; set; }
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
         
        public bool  IsActive { get; set; }
    }
}
