using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AccountingBlueBook.Configuration;
using AccountingBlueBook.EntityFrameworkCore;
using AccountingBlueBook.Migrator.DependencyInjection;

namespace AccountingBlueBook.Migrator
{
    [DependsOn(typeof(AccountingBlueBookEntityFrameworkModule))]
    public class AccountingBlueBookMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public AccountingBlueBookMigratorModule(AccountingBlueBookEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(AccountingBlueBookMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                AccountingBlueBookConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AccountingBlueBookMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
