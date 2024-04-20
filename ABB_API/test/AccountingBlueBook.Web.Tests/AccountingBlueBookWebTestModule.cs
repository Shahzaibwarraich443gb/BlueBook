using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AccountingBlueBook.EntityFrameworkCore;
using AccountingBlueBook.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace AccountingBlueBook.Web.Tests
{
    [DependsOn(
        typeof(AccountingBlueBookWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class AccountingBlueBookWebTestModule : AbpModule
    {
        public AccountingBlueBookWebTestModule(AccountingBlueBookEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AccountingBlueBookWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(AccountingBlueBookWebMvcModule).Assembly);
        }
    }
}