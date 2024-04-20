using Abp.Domain.Entities.Auditing;
using Castle.Components.DictionaryAdapter;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    public class DLState:FullAuditedEntity
    {
        public string StateName { get; set; }

        public int StateCode { get; set; }
    }
}
