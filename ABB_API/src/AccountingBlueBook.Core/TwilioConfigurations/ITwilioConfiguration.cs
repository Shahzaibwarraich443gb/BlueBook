using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.TwilioConfiguratrions
{
    public interface ITwilioConfiguration : ITransientDependency
    {
        string accountSid { get; }

        string authToken { get; }

        string fromPhoneNumber { get; }
    }
}
