using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.Main;

namespace AccountingBlueBook.AppServices.ContactPersonTypes.Dto
{


    public class ContactPersonTypeDto : EntityDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        
    }
}
