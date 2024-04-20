using System.Collections.Generic;
using System.IO;
using System;

namespace AccountingBlueBook.Authorization.Users
{
    public class EmailDto
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ToEmail { get; set; }
        public List<Tuple<string, MemoryStream>> Streams { get; set; }
    }
}