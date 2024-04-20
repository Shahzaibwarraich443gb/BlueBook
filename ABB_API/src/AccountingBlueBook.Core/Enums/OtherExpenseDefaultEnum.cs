using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Enums
{
    public enum OtherExpenseDefaultEnum
    {
        [Display(Name = "Compensation of officer")]
        CompensationOfOfficer,

        [Display(Name = "Salaries and wages")]
        SalariesAndWages,

        [Display(Name = "Repair and maintenance")]
        RepairAndMaintenance,

        [Display(Name = "Insurance premium")]
        InsurancePremium,

        [Display(Name = "Depreciation")]
        Depreciation,

        [Display(Name = "Advertising")]
        Advertising,

        [Display(Name = "Taxes and licenses")]
        TaxesAndLicenses,

        [Display(Name = "Telephone and internet")]
        TelephoneAndInternet,

        [Display(Name = "Auto expenses")]
        AutoExpenses,

        [Display(Name = "Bank charges")]
        BankCharges,

        [Display(Name = "Delivery and shipping")]
        DeliveryAndShipping,

        [Display(Name = "Printing and stationary")]
        PrintingAndStationary,

        [Display(Name = "Computer expenses")]
        ComputerExpenses
    }

}
