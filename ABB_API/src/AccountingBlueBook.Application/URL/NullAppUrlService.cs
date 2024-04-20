using AccountingBlueBook.URL;

using System;

namespace AccountingBlueBook.URL
{
    public class NullAppUrlService : IAppUrlService
    {
       public static IAppUrlService Instance { get; } = new NullAppUrlService();

        private NullAppUrlService()
        {

        }

        public string CreateEmailActivationUrlFormatForTenant(int? tenantId)
        {
            throw new NotImplementedException();
        }

        public string CreatePasswordResetUrlFormatForTenant(int? tenantId)
        {
            throw new NotImplementedException();
        }

        public string CreateEmailActivationUrlFormat(string tenancyName)
        {
            throw new NotImplementedException();
        }

        public string CreatePasswordResetUrlFormat(string tenancyName)
        {
            throw new NotImplementedException();
        }
    }
}
