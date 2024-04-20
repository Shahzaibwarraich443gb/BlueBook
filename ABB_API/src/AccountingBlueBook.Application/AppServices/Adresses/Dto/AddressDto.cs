using Abp.Application.Services.Dto;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Adresses.Dto
{
    public class AddressDto : EntityDto
    {
        public string CompleteAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }
        public string Fax { get; set; }
        public string Type { get; set; }
        public bool IsPrimary { get; set; }
    }
}
