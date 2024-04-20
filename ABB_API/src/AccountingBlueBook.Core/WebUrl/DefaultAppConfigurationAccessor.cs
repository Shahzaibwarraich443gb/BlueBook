using Abp.Dependency;
using AccountingBlueBook.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingBlueBook.WebUrl;

namespace AccountingBlueBook.WebUrl
{
    public class DefaultAppConfigurationAccessor : IAppConfigurationAccessor, ISingletonDependency
    {
        public IConfigurationRoot Configuration { get; }

        public DefaultAppConfigurationAccessor()
        {
            Configuration = AppConfigurations.Get(Directory.GetCurrentDirectory());
        }
    }
}
