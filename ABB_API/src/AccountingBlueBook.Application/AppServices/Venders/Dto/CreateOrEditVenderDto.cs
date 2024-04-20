using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Venders.Dto
{
    public class CreateOrEditVenderDto : EntityDto
    {
        public string BusinessName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public long TaxId { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string SSN { get; set; }
        public string VendorName { get; set; }
        public int? VenderTypeId { get; set; }
        public int CompanyId { get; set; }
    }
}
