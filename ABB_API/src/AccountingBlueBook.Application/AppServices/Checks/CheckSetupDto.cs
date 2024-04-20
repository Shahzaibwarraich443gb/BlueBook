using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Checks
{
    public class CheckSetupDto: FullAuditedEntityDto<long>
    {
        public string CheckStyle { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public long BankId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public int FirstFooter { get; set; }
        public int SecondFooter { get; set; }
        public int ThirdFooter { get; set; }

    }
}
