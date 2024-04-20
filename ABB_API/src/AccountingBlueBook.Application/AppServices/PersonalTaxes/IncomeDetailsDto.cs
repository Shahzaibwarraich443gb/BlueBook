using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.PersonalTaxes
{
    public class IncomeDetailsDto: FullAuditedEntityDto<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string IncomeDescription { get; set; }
        public double Amount { get; set; }
        public int FederalWH { get; set; }
        public int StateWH { get; set; }
    }
}
