using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.MainEntities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.ContactDetails.Dto
{
    public  class GeneralContactDetailsDto : EntityDto
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
