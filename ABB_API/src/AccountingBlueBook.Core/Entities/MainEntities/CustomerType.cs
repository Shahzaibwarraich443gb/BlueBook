﻿using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    [Table("CustomerTypes")]
    public class CustomerType : FullAuditedEntity
    {
        public string Name { get; set; }
       public bool IsActive { get; set; }
    }
}
