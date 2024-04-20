using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities.Reports.Dto
{
    public class AuditLogsDto : EntityDto
    {
        public string AppLog { get; set; }
        public DateTime? LogDate { get; set; }
        public string Operation { get; set; }
        public string EmployeeName { get; set; }
        public string CustomerName { get; set; }
        public string Company { get; set; }
        public long? CustomerNo { get; set; }

        public string CompanyName { get; set; }

        public string UserName { get; set; }

        public int UserId { get; set; }
    }
}
