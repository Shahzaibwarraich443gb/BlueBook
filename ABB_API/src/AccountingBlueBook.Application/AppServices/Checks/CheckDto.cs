using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Checks
{
    public class CheckDto: FullAuditedEntityDto<long>
    {
        public string CheckCode { get; set; }
        public string PayeeId { get; set; }
        public virtual Customer Payee { get; set; }
        public double TotalAmount { get; set; }
        public long BankId { get; set; }
        public virtual Bank Bank { get; set; }
        public string Notes { get; set; }
        public long CompanyId { get; set; }

        public List<CheckProductDetail> CheckProductDetails { get; set; }
        public List<CheckAccountDetail> CheckAccountDetails { get; set; }
    }

    public class PayeeDto: EntityDto<string>
    {
        public string PayeeName { get; set; }
        public string PayeeType { get; set; }

    }
}
