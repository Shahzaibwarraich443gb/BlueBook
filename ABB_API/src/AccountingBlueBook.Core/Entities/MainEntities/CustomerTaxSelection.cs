using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities
{
    public class CustomerTaxSelection: Entity<long>
    {
        public int CustomerId { get; set; }
        public TaxService TaxService { get; set; }
    }
}
