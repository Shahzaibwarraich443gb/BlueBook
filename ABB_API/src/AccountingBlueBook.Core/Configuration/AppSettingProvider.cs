using System.Collections.Generic;
using System.Linq;
using Abp.Configuration;
using Abp.Net.Mail;
using AccountingBlueBook.WebUrl;
using Microsoft.Extensions.Configuration;

namespace AccountingBlueBook.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        private readonly IConfigurationRoot _appConfiguration;

        public AppSettingProvider(IAppConfigurationAccessor configurationAccessor)
        {
            _appConfiguration = configurationAccessor.Configuration;
        }
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            ChangeEmailSettingScopes(context);
            return GetHostSettings().Union(GetTenantSettings()).Union(GetSMTPSettings()).Union(GetThemeSettings());

            //return new[]
            //{
            //    new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, clientVisibilityProvider: new VisibleSettingClientVisibilityProvider())
            //};
        }
        private IEnumerable<SettingDefinition> GetThemeSettings()
        {
            return new[] {
                new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, isVisibleToClients: true),
           };
        }
        private IEnumerable<SettingDefinition> GetTenantSettings()
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.Email.UseHostDefaultEmailSettings, GetFromAppSettings(AppSettingNames.Email.UseHostDefaultEmailSettings, AccountingBlueBookConsts.MultiTenancyEnabled? "true":"false"), scopes: SettingScopes.Tenant)
            };
        }
        private void ChangeEmailSettingScopes(SettingDefinitionProviderContext context)
        {
            if (!AccountingBlueBookConsts.AllowTenantsToChangeEmailSettings)
            {
                context.Manager.GetSettingDefinition(EmailSettingNames.Smtp.Host).Scopes = SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.Smtp.Port).Scopes = SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.Smtp.UserName).Scopes = SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.Smtp.Password).Scopes = SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.Smtp.Domain).Scopes = SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.Smtp.EnableSsl).Scopes = SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.Smtp.UseDefaultCredentials).Scopes = SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.DefaultFromAddress).Scopes = SettingScopes.Application;
                context.Manager.GetSettingDefinition(EmailSettingNames.DefaultFromDisplayName).Scopes = SettingScopes.Application;
            }
        }
        private IEnumerable<SettingDefinition> GetSMTPSettings()
        {
            return new[] {
                 new SettingDefinition(EmailSettingNames.Smtp.UserName, "info@employeemonitorsystem.com", scopes: SettingScopes.All, isVisibleToClients: true),
                 new SettingDefinition(EmailSettingNames.Smtp.Password, "N><Aa8y6g", scopes: SettingScopes.All, isVisibleToClients: true),
                 new SettingDefinition(EmailSettingNames.Smtp.Port, "587", scopes: SettingScopes.All, isVisibleToClients: true),
                 new SettingDefinition(EmailSettingNames.Smtp.Host, "smtp.gmail.com", scopes: SettingScopes.All, isVisibleToClients: true),
                 new SettingDefinition(EmailSettingNames.DefaultFromAddress, "info@eworkforcepayroll.com", scopes: SettingScopes.All, isVisibleToClients: true),
                 new SettingDefinition(EmailSettingNames.DefaultFromDisplayName, "DefaultFromDisplayName", scopes: SettingScopes.All, isVisibleToClients: true),
            };
        }
        private IEnumerable<SettingDefinition> GetHostSettings()
        {
            return new[] {
                new SettingDefinition(AppSettingNames.TenantManagement.AllowSelfRegistration, GetFromAppSettings(AppSettingNames.TenantManagement.AllowSelfRegistration, "true"), isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.TenantManagement.IsNewRegisteredTenantActiveByDefault, GetFromAppSettings(AppSettingNames.TenantManagement.IsNewRegisteredTenantActiveByDefault, "false")),
                new SettingDefinition(AppSettingNames.TenantManagement.UseCaptchaOnRegistration, GetFromAppSettings(AppSettingNames.TenantManagement.UseCaptchaOnRegistration, "true"), isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.TenantManagement.DefaultEdition, GetFromAppSettings(AppSettingNames.TenantManagement.DefaultEdition, "")),
                new SettingDefinition(AppSettingNames.TenantManagement.SubscriptionExpireNotifyDayCount, GetFromAppSettings(AppSettingNames.TenantManagement.SubscriptionExpireNotifyDayCount, "7"), isVisibleToClients: true),
                //new SettingDefinition(AppSettingNames.HostManagement.BillingLegalName, GetFromAppSettings(AppSettingNames.HostManagement.BillingLegalName, "")),
                //new SettingDefinition(AppSettingNames.HostManagement.BillingAddress, GetFromAppSettings(AppSettingNames.HostManagement.BillingAddress, "")),
            };
        }

        private string GetFromAppSettings(string name, string defaultValue = null)
        {
            return GetFromSettings("App:" + name, defaultValue);
        }

        private string GetFromSettings(string name, string defaultValue = null)
        {
            return _appConfiguration[name] ?? defaultValue;
        }
    }
}
