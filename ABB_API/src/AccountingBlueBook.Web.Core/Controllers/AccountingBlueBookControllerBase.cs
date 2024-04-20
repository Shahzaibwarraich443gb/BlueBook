using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace AccountingBlueBook.Controllers
{
    public abstract class AccountingBlueBookControllerBase: AbpController
    {
        protected AccountingBlueBookControllerBase()
        {
            LocalizationSourceName = AccountingBlueBookConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
