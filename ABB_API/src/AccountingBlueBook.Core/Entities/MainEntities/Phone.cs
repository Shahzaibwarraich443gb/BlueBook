using Abp.Domain.Entities.Auditing;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.Main
{
    [Table("PhoneNumbers")]
    public class Phone : FullAuditedEntity
    {
        public PhoneType Type { get; set; }
        public string Number { get; set; }
        public bool IsPrimary { get; set; }
    }
}
