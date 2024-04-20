using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AccountingBlueBook.Configuration;

namespace AccountingBlueBook.Web.Host.Startup
{
    [DependsOn(
       typeof(AccountingBlueBookWebCoreModule))]
    public class AccountingBlueBookWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AccountingBlueBookWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AccountingBlueBookWebHostModule).GetAssembly());
        }
    }
}
