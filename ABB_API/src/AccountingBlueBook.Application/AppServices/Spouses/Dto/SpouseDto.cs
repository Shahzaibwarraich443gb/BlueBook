using Abp.Application.Services.Dto;
using System;

namespace AccountingBlueBook.AppServices.Spouses.Dto
{
    public  class SpouseDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string SSN { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string SpouseJobDescription { get; set; }
        public string DrivingLicense { get; set; }
        public DateTime? DLIssue { get; set; }
        public DateTime? DLExpiry { get; set; }
        public int? DLState { get; set; }
        public int Code { get; set; }
        public int customerId { get; set; } = 0;
        public bool? IsActive { get; set; }
        public int? JobTitleId { get; set; }

        public string SpouseSuffix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }    

        public int? CompanyId { get; set; }
        
        public int? EthnicityId { get; set; }
        
        public int? LanguageId { get; set; }
       
    }
}
