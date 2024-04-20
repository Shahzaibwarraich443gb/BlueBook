using Abp.Application.Services.Dto;

namespace AccountingBlueBook.AppServices.ContactPersonTypes.Dto
{
    public class CreateOrEditContactPersonTypeInputDto : EntityDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int? CompanyId { get; set; }
    }
}
