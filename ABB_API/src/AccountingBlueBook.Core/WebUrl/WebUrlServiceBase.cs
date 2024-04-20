using Abp.Extensions;
using Microsoft.Extensions.Configuration;

namespace AccountingBlueBook.WebUrl
{

    public abstract class WebUrlServiceBase : IWebUrlServiceBase
    {
        public const string TenancyNamePlaceHolder = "{TENANCY_NAME}";
        public abstract string WebSiteRootAddressFormatKey { get; }

        public abstract string ServerRootAddressFormatKey { get; }

        public string WebSiteRootAddressFormat => _appConfiguration[WebSiteRootAddressFormatKey] ?? "http://192.168.1.57:4200/";

        public string ServerRootAddressFormat => _appConfiguration[ServerRootAddressFormatKey] ?? "http://192.168.1.57:4200/";


        readonly IConfigurationRoot _appConfiguration;

        public WebUrlServiceBase(IAppConfigurationAccessor configurationAccessor)
        {
            _appConfiguration = configurationAccessor.Configuration;
        }

        public string GetSiteRootAddress(string tenancyName = null)
        {
            return ReplaceTenancyNameInUrl(WebSiteRootAddressFormat, tenancyName);
        }

        public string GetServerRootAddress(string tenancyName = null)
        {
            return ReplaceTenancyNameInUrl(ServerRootAddressFormat, tenancyName);
        }

        private string ReplaceTenancyNameInUrl(string siteRootFormat, string tenancyName)
        {
            if (!siteRootFormat.Contains(TenancyNamePlaceHolder))
            {
                return siteRootFormat;
            }

            if (siteRootFormat.Contains(TenancyNamePlaceHolder + "."))
            {
                siteRootFormat = siteRootFormat.Replace(TenancyNamePlaceHolder + ".", TenancyNamePlaceHolder);
            }

            if (tenancyName.IsNullOrEmpty())
            {
                return siteRootFormat.Replace(TenancyNamePlaceHolder, "");
            }

            return siteRootFormat.Replace(TenancyNamePlaceHolder, tenancyName + ".");
        }
    }

}
