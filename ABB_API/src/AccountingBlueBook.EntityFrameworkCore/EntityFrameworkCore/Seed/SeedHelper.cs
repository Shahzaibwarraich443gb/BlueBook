using System;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Uow;
using Abp.MultiTenancy;
using AccountingBlueBook.EntityFrameworkCore.Seed.Host;
using AccountingBlueBook.EntityFrameworkCore.Seed.Tenants;
using System.Linq;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Migrations;

namespace AccountingBlueBook.EntityFrameworkCore.Seed
{
    public static class SeedHelper
    {
        public static void SeedHostDb(IIocResolver iocResolver)
        {
            WithDbContext<AccountingBlueBookDbContext>(iocResolver, SeedHostDb);
        }

        public static void SeedHostDb(AccountingBlueBookDbContext context)
        {
            context.SuppressAutoSetTenantId = true;

            // Host seed
            new InitialHostDbBuilder(context).Create();

            // Default tenant seed (in host database).
            new DefaultTenantBuilder(context).Create();
            new TenantRoleAndUserBuilder(context, 1).Create();

            //add Seeding
            SeedDLStates(context);
            SeedLegalStatus(context);
            SeedTenureForm(context);
        }

        private static void WithDbContext<TDbContext>(IIocResolver iocResolver, Action<TDbContext> contextAction)
            where TDbContext : DbContext
        {
            using (var uowManager = iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin(TransactionScopeOption.Suppress))
                {
                    var context = uowManager.Object.Current.GetDbContext<TDbContext>(MultiTenancySides.Host);

                    contextAction(context);

                    uow.Complete();
                }
            }
        }

        private static void SeedDLStates(AccountingBlueBookDbContext context)
        {
            if (!context.DLStates.Any())
            {
                var dlStates = new[]
                {
           new DLState { StateCode = 120, StateName = "Alabama" },
           new DLState { StateCode = 121, StateName = "Alaska" },
           new DLState { StateCode = 122, StateName = "American Samoa" },
           new DLState { StateCode = 123, StateName = "Arizona" },
           new DLState { StateCode = 124, StateName = "Arkansas" },
           new DLState { StateCode = 125, StateName = "California" },
           new DLState { StateCode = 126, StateName = "Colorado" },
           new DLState { StateCode = 127, StateName = "Connecticut" },
           new DLState { StateCode = 128, StateName = "Delaware" },
           new DLState { StateCode = 129, StateName = "District of Columbia" },
           new DLState { StateCode = 130, StateName = "Federated States of Micronesia" },
           new DLState { StateCode = 131, StateName = "Florida" },
           new DLState { StateCode = 132, StateName = "Georgia" },
           new DLState { StateCode = 133, StateName = "Guam" },
           new DLState { StateCode = 134, StateName = "Hawaii" },
           new DLState { StateCode = 135, StateName = "Idaho" },
           new DLState { StateCode = 136, StateName = "Illinois" },
           new DLState { StateCode = 137, StateName = "Indiana" },
           new DLState { StateCode = 138, StateName = "Iowa" },
           new DLState { StateCode = 139, StateName = "Kansas" },
           new DLState { StateCode = 140, StateName = "Kentucky" },
           new DLState { StateCode = 141, StateName = "Louisiana" },
           new DLState { StateCode = 142, StateName = "Maine" },
           new DLState { StateCode = 143, StateName = "Marshall Islands" },
           new DLState { StateCode = 144, StateName = "Maryland" },
           new DLState { StateCode = 145, StateName = "Massachusetts" },
           new DLState { StateCode = 146, StateName = "Michigan" },
           new DLState { StateCode = 147, StateName = "Minnesota" },
           new DLState { StateCode = 148, StateName = "Mississippi" },
           new DLState { StateCode = 149, StateName = "Missouri" },
           new DLState { StateCode = 150, StateName = "Montana" },
           new DLState { StateCode = 151, StateName = "Nebraska" },
           new DLState { StateCode = 152, StateName = "Nevada" },
           new DLState { StateCode = 153, StateName = "New Hampshire" },
           new DLState { StateCode = 154, StateName = "New Jersey" },
           new DLState { StateCode = 155, StateName = "New Mexico" },
           new DLState { StateCode = 156, StateName = "New York" },
           new DLState { StateCode = 157, StateName = "North Carolina" },
           new DLState { StateCode = 158, StateName = "North Dakota" },
           new DLState { StateCode = 159, StateName = "Northern Mariana Islands" },
           new DLState { StateCode = 160, StateName = "Ohio" },
           new DLState { StateCode = 161, StateName = "Oklahoma" },
           new DLState { StateCode = 162, StateName = "Oregon" },
           new DLState { StateCode = 163, StateName = "Palau" },
           new DLState { StateCode = 164, StateName = "Pennsylvania" },
           new DLState { StateCode = 165, StateName = "Puerto Rico" },
           new DLState { StateCode = 166, StateName = "Rhode Island" },
           new DLState { StateCode = 167, StateName = "South Carolina" },
           new DLState { StateCode = 168, StateName = "South Dakota" },
           new DLState { StateCode = 169, StateName = "Tennessee" },
           new DLState { StateCode = 170, StateName = "Texas" },
           new DLState { StateCode = 171, StateName = "Utah" },
           new DLState { StateCode = 172, StateName = "Vermont" },
           new DLState { StateCode = 173, StateName = "Virgin Islands" },
           new DLState { StateCode = 174, StateName = "Virginia" },
           new DLState { StateCode = 175, StateName = "Washington" },
           new DLState { StateCode = 176, StateName = "West Virginia" },
           new DLState { StateCode = 177, StateName = "Wisconsin" },
           new DLState { StateCode = 178, StateName = "Wyoming" }

        };

                context.DLStates.AddRange(dlStates);
                context.SaveChanges();
            }
        }

        private static void SeedLegalStatus(AccountingBlueBookDbContext context)
        {
            if (!context.LegalStatuses.Any())
            {
                var legalStatus = new[]
                {
                    new LegalStatus{Name = "Sole Properietor"},
                    new LegalStatus{Name = "PartnerShip"},
                    new LegalStatus{Name = "Association"},
                    new LegalStatus{Name = "C Corp"},
                    new LegalStatus{Name = "Et AI"}
                };


                context.LegalStatuses.AddRange(legalStatus);
                context.SaveChanges();

            }
        }

        private static void SeedTenureForm(AccountingBlueBookDbContext context)
        {

            if (!context.TenureForms.Any())
            {
                var tenureForms = new[]
                {
                    new TenureForm{Name = "Annualy (ST 101)"},
                    new TenureForm{Name = "Quaterly (ST 100)"}
                };


                context.TenureForms.AddRange(tenureForms);
                context.SaveChanges();

            }
        }
    }

}
