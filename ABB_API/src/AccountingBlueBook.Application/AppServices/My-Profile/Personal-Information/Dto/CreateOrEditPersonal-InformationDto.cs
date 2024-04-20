using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.MainEntities;
using System;

namespace AccountingBlueBook.AppServices.PersonalInformations.Dto
{
    public class CreateOrEditPersonalInformationInputDto :  EntityDto
    {
        public Title Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public Gender Gender { get; set; }

        public DateTime? DateofBirth { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime? HireDate { get; set; }
        public int DefaultSessionTimeout { get; set; }
        public int TenantId { get; set; }
    }
}