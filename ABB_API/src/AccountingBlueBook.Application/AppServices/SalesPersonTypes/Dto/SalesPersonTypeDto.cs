using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.SalesPersonTypes.Dto
{
    public class SalesPersonTypeDto : EntityDto
    {
        public string Name { get; set; }
        public string Email { get; set; }   
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public int PhoneId { get; set; } 
        public int AddressId { get; set; }
        public string compeleteAddress { get;set; }
        public int? CompanyId { get; set; }
        public string  PhoneNumber { get; set; }
        public Phone Phone { get; set; }
        public Address Address { get; set; }
        //public Company Company { get; set; }
    }
}
