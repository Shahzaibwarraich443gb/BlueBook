using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Enums
{
    public class EstimateEnum
    {
        public enum EstimateType
        {
            Invoice = 1,
            Estimate = 2,
            Credit_Note = 3,
            Sale_Receipt = 4,
            Receive_Payment = 5,
            Expense = 6,
            Purchase_Receipt = 7,
            Recurring_Invoice = 8,
            Check = 9,
            Purchase_Invoice = 10,
            Purchase_Payment = 11,
        }
        public enum EstimateStatus
        {
            Open = 1,
            Paid = 2,
            Closed = 3,
            Partial = 4
        }
    }
}
