using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    [Table("CalendarEvents")]
    public class CalendarEvent : Entity<long>
    {
        public string refModuleId { get; set; }
        public string refReference { get; set; }
        public long? refUserId { get; set; }
        public long? refCompanyId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Meridian1 { get; set; }
        public string Meridian2 { get; set; }
        public int? Priority { get; set; }
        public string ThemeColor { get; set; }
        public int? AlertBefore { get; set; }
        public DateTime? AlertBeforeTime { get; set; }
        public bool? IsAlertBefore { get; set; }
        public int? SnoozeTime { get; set; }
        public bool? IsSnooze { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsDeleted { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreationTime { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool? IsRepeat { get; set; }
        public int? RepeatType { get; set; }
        public bool? ISUPCOMINGEVENT { get; set; }
    }
}
