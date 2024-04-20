using Abp.Application.Services.Dto;
using System;

namespace AccountingBlueBook.AppServices.Spouses.Dto
{
    public  class CreateOrEditSpouseDto : EntityDto<long>
    {
        public string SpouseSuffix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string SSN { get; set; }
        public string SpouseJobDescription { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? LanguageId { get; set; }
        public int? JobTitleId { get; set; }
        public int? EthnicityId { get; set; }

        public bool IsActive { get; set; }
    }
}
