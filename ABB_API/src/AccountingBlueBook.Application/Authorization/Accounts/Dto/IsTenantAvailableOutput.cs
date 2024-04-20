namespace AccountingBlueBook.Authorization.Accounts.Dto
{
    public class IsTenantAvailableOutput
    {
        public TenantAvailabilityState State { get; set; }

        public int? TenantId { get; set; }
        public bool IsPaidTenant { get; set; }

        public IsTenantAvailableOutput()
        {
        }

        public IsTenantAvailableOutput(TenantAvailabilityState state, int? tenantId = null)
        {
            State = state;
            TenantId = tenantId;
        }

        public IsTenantAvailableOutput(TenantAvailabilityState state, int tenantId, bool isPaidTenant)
        {
            State = state;
            TenantId = tenantId;
            IsPaidTenant = isPaidTenant;
        }

    }
}
