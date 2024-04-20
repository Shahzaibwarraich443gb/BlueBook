using System.ComponentModel.DataAnnotations;
using Abp.MultiTenancy;

namespace AccountingBlueBook.Authorization.Accounts.Dto
{
    public class IsTenantAvailableInput
    {
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }

        public string EmailAddress { get; set; }
    }
}
