using Abp.Extensions;
using AccountingBlueBook.WebUrl;
using Microsoft.Extensions.Configuration;

using System.Collections.Generic;

namespace AccountingBlueBook.TwilioConfiguratrions
{
    public class TwilioConfiguration : ITwilioConfiguration
    {
        private readonly IConfigurationRoot _appConfiguration;

        //public SubscriptionPaymentGatewayType GatewayType => SubscriptionPaymentGatewayType.Stripe;

        //public string accountSid => _appConfiguration["Twillio:accountSid"].EnsureEndsWith('/');
        public string accountSid => _appConfiguration["Twillio:accountSid"];
        public string authToken => _appConfiguration["Twillio:authToken"];

        public string fromPhoneNumber => _appConfiguration["Twillio:fromPhoneNumber"];

        //public string WebhookSecret => _appConfiguration["Payment:Stripe:WebhookSecret"];

        //public bool IsActive => _appConfiguration["Payment:Stripe:IsActive"].To<bool>();

        //public bool SupportsRecurringPayments => true;

        //public List<string> PaymentMethodTypes => _appConfiguration.GetSection("Payment:Stripe:PaymentMethodTypes").Get<List<string>>();

        public TwilioConfiguration(IAppConfigurationAccessor configurationAccessor)
        {
            _appConfiguration = configurationAccessor.Configuration;
        }
    }

}
