using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.CoreModel
{

    public enum FilingRateType
    {
        FederalFilingFee=1,
        StateFilingFee=2,
        OtherFee=3
    }
    [Table("FilingRates")]
    public class FilingRate : FullAuditedEntity<long>
    {
        public long? FormId { get; set; }
        public string FormName { get; set; }
        [Column(TypeName = "decimal(18,2)")]
       public decimal EFileRate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal MailRate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool Active { get; set; }
        public int Year { get; set; }
        public FilingRateType FilingRateType {get;set;}
       
    }
}
