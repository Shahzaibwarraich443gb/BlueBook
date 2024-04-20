using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities.Reports.Dto
{
    public class BalanceSheetDto: EntityDto
    {
        public string AccountTypeName { get; set; }
        public string AccountDescription { get; set; }
        public decimal? AccountBalance { get; set; }
        public long? AccountTypeID { get; set; }
    }
}
