using Abp.AutoMapper;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AccountingBlueBook.Authorization;

namespace AccountingBlueBook
{
    [DependsOn(
        typeof(AccountingBlueBookCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class AccountingBlueBookApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<AccountingBlueBookAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(AccountingBlueBookApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
