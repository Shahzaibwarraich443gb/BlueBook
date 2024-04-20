using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.DLStates.Dto
{
    public class DLStateDto:FullAuditedEntityDto
    {
        public string StateName { get; set; }
         
        public string StateCode { get; set; }
    }
}
