using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Enums
{
    public enum IncomeDescriptionEnum
    {
        [EnumMember(Value = "W-2")]
        W2 = 1, 
        [EnumMember(Value = "W-3")]
        W3 = 2,
        [EnumMember(Value = "W-4")]
        W4 = 3,
    }
}
