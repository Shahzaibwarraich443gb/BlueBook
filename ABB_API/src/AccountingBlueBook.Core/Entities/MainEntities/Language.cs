using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.Main
{
    [Table("Languages")]
    public class Language : FullAuditedEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public bool IsActive { get; set; }
        public string Description { get; set; }
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
    }
}
