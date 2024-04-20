using System.Runtime.ExceptionServices;

namespace AccountingBlueBook.MultiTenancy
{
    public class InputDto
    {
        public  string CompanyName {get;set;}
       public string FirstName { get; set; }
       public string lastname { get; set; }
        public string address { get; set; }
        public string City { get; set; }
        public string ZipCode{ get; set; }
        public string Country { get; set; }
        public string State { get; set; }

        public string suffix { get;set; }

        public int PhoneNumber { get; set; }

        public string EmailAddress { get; set; }
    }
}