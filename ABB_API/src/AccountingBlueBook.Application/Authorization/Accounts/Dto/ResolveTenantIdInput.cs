using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Authorization.Accounts.Dto
{
    public class ResolveTenantIdInput
    {
        // An encrypted text which contains tenantId={value} string
        public string c { get; set; }
    }
}
