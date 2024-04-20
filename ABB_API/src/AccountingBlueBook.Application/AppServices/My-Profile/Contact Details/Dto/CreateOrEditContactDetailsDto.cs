using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.MainEntities;
using System;

namespace AccountingBlueBook.AppServices.ContactDetails.Dto
{
    public class CreateOrEditContactDetalsInputDto :  EntityDto
    {
        public string Email { get; set; }

        public int PhoneNumber { get; set; }

        public int MobileNumber { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }


        public int TenantId { get; set; }
    }
}