using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Enums
{
    public enum CostOfSaleDefaultEnum
    {
        [Display(Name = "Beginning Inventory")]
        BeginningInventory = 1,

        [Display(Name = "Purchase")]
        Purchase = 2,

        [Display(Name = "Cost of Labor")]
        CostOfLabor = 3,

        [Display(Name = "Closing Inventory")]
        ClosingInventory = 4,
    }
}
