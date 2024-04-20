using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Common.CommonLookupDto
{
    public class CommonLookupInput<T> where T : class
    {
        public T Item { get; set; }
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
    }
}
