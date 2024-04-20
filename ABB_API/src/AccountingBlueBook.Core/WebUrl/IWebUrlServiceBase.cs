using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.WebUrl
{
    public interface IWebUrlServiceBase
    {
        string WebSiteRootAddressFormat { get; }

        string ServerRootAddressFormat { get; }

        string GetSiteRootAddress(string tenancyName = null);

        string GetServerRootAddress(string tenancyName = null);

    }
}
