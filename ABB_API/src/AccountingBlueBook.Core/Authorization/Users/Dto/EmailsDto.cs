using System;
using System.Collections.Generic;
using System.IO;

namespace AccountingBlueBook.Authorization.Users.Dto
{   
    public class EmailsDto
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ToEmail { get; set; }
        public List<Tuple<string,MemoryStream>> Streams { get; set; }
    }
}
