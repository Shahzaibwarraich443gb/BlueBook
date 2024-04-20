using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.MainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Venders.Dto
{
    public  class VendorDto : EntityDto
    {
        public string BusinessName { get; set; }

        public long TaxId { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string SSN { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? VenderTypeId { get; set; }       
        public VenderType Vender { get; set; }
    }
}
