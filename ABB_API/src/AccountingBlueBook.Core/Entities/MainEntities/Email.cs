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
    [Table("Emails")]
    public class Email : FullAuditedEntity
    {
        public EmailType TypeEmail { get; set; }
        public string EmailAddress { get; set; }
        public bool IsPrimary { get; set; }
        public bool EmailVerified { get; set; }

    }
}
