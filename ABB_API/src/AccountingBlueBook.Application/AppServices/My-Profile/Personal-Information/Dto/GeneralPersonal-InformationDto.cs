using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.MainEntities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.PersonalInformations.Dto
{
    public  class GeneralPersonalInformationDto : EntityDto
    {
        public Title Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }

        public int PhoneNumber { get; set; }

        public int MobileNumber { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }

        public DateTime? DateofBirth { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime? HireDate { get; set; }
        public int DefaultSessionTimeout { get; set; }
        public int TenantId { get; set; }
    }
}
