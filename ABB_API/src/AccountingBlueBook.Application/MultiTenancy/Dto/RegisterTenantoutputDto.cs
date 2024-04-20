using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.MultiTenancy;

namespace AccountingBlueBook.MultiTenancy.Dto
{
    //[AutoMapTo(typeof(Tenant))]
    public class RegisterTenantoutputDto
    {
        public int TenantId { get; set; }

        public string TenancyName { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public bool IsTenantActive { get; set; }

        public bool IsActive { get; set; }

        public bool IsEmailConfirmationRequired { get; set; }
    }
}
