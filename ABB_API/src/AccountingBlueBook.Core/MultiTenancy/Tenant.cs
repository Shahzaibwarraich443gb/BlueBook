using Abp.MultiTenancy;
using AccountingBlueBook.Authorization.Users;
using System;

namespace AccountingBlueBook.MultiTenancy
{
    public enum SubscriptionPaymentType
    {
        Manual = 0,
        RecurringAutomatic = 1,
        RecurringManual = 2
    }
    public class Tenant : AbpTenant<User>
    {

      
        public Tenant()
        {            
        }


        public Guid TenantKey { get; set; }

        public SubscriptionPaymentType SubscriptionPaymentType { get; set; }
        public DateTime? SubscriptionEndDateUtc { get; set; }
        public bool IsInTrialPeriod { get; set; }

        public string TimeZone { get; set; }
        public string UtcOffSet { get; set; }





        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
