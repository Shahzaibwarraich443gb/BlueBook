using Abp;
using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook
{

    public abstract class AccountingBlueBook_Web_ApplicationServiceBase : AbpServiceBase
    {
        protected AccountingBlueBook_Web_ApplicationServiceBase()
        {
            LocalizationSourceName = AccountingBlueBookConsts.LocalizationSourceName;
        }
    }
}
