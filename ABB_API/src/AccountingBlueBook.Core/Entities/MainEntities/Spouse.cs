using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingBlueBook.Entities.Main
{
    [Table("Spouses")]

    public class Spouse : FullAuditedEntity<long>
    {
        public string Name { get; set; }
        public string SSN { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string SpouseJobDescription { get; set; }
        public string? Occupation { get; set; }
        public string DrivingLicense { get; set; }
        public DateTime? DLIssue { get; set; }
        public DateTime? DLExpiry { get; set; }
        public int? DLState { get; set; }
        public int? Code { get; set; }
        //Old Code
        public string SpouseSuffix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public int? JobTitleId { get; set; }
        [ForeignKey("JobTitleId")]
        public JobTitle JobTitle { get; set; }

        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        public int? EthnicityId { get; set; }
        [ForeignKey("EthnicityId")]
        public Ethnicity Ethnicity { get; set; }

        public int? LanguageId { get; set; }
        [ForeignKey("LanguageId")]
        public Language Language { get; set; }
    }
}
