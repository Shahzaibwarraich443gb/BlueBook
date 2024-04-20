using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum OtherIncomeDefaultEnum
    {
        [Display(Name = "Gross Rent")]
        GrossRent = 1,

        [Display(Name = "Capital Gain Net Income")]
        CapitalGainNetIncome = 2,

        [Display(Name = "Net Gain or Loss Form 42")]
        NetGainOrLossForm42 = 3,
    }

}
