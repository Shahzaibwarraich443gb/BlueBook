using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Enums
{
    public enum CheckFooterEnum
    {
        [Display(Name = "Check No")]
        CheckNo = 1,
        [Display(Name = "Routing No")]
        RoutingNo = 2,
        [Display(Name = "Account No")]
        AccountNo = 3
    }
}
