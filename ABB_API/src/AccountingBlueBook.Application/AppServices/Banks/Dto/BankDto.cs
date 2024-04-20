using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Banks.Dto
{
    public  class BankDto  : EntityDto
    {
        public string TitleofAccount { get; set; }
        public string BankName { get; set; }
        public int AccountNumber { get; set; }
        public int Routing { get; set; }
        public decimal OpenBalance { get; set; }
        public int StartingCheque { get; set; }
        public int SwiftCode { get; set; }
        public bool IsActive { get; set; }
        public int? BankAddressId { get; set; }
        public long CoaMainHeadId { get; set; }
        public CreateOrEditBankAddressDto Address { get; set; }

    }
}
