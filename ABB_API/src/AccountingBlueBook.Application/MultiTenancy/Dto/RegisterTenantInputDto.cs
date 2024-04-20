using System;
using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.MultiTenancy;

namespace AccountingBlueBook.MultiTenancy.Dto
{
    public enum SubscriptionStartType
    {
        Free = 1,
        Trial = 2,
        Paid = 3
    }
    public enum PaymentPeriodType
    {
        Daily = 1,
        Weekly = 7,
        Monthly = 30,
        Annual = 365
    }

    // [AutoMapTo(typeof(Tenant))]
    public class RegisterTenantInputDto
    {
      
        public string TenancyName { get; set; }

        // [Required]
        public string Name { get; set; }

       // [Required]
  //      [EmailAddress]
        public string AdminEmailAddress { get; set; }

        //MYFormData 
        public string FirstName { get; set; } 
        public string lastName { get; set; }

        public string Suffix { get; set; }
       
        public string AdminPhoneNumber { get; set; }



        public string PhoneNo { get; set; }

        public int NoOfUsers { get; set; }

        public int Storagelimit { get; set; }

      

        public DateTime FiscalYearStart { get; set; }
        public DateTime FiscalYearEnd { get; set; }

        public string Address { get; set; }

       public bool IsAccept { get; set; }
        public string Zip_code { get; set; }

        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }

        //


        public string AdminPassword { get; set; }

        
        public string CaptchaResponse { get; set; }

        public SubscriptionStartType SubscriptionStartType { get; set; }

        public int? EditionId { get; set; }
        public bool IsCustomEdition { get; set; }
    
        public PaymentPeriodType PaymentPeriodType { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        
        public string VarifiedPhoneNo { get; set; }


    }
}
