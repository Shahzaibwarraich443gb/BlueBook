using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Extensions;
using AccountingBlueBook.Validation;

namespace AccountingBlueBook.Authorization.Accounts.Dto
{
    public class RegisterInput : IValidatableObject
    {


      
      
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        public string companyName { get; set; }
        public string FirstName { get; set; }

        public string lastName { get; set; }

        public int PhoneNumber { get; set; }

        public string noofusers { get; set; }
        public bool IsAccept { get; set; }

        public string storagelim { get; set; }
        public DateTime FiscalYearStart { get; set; }
        public DateTime FiscalYearEnd { get; set; }
        public bool twowayauthentication { get; set; }

        
        [StringLength(AbpUserBase.MaxSurnameLength)]
  
        public string Surname { get; set; }
        public string Address { get; set; }
             public string Zip_code { get; set; }

            public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }






       
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        
        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

       

        [DisableAuditing]
        public string CaptchaResponse { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!UserName.IsNullOrEmpty())
            {
                if (!UserName.Equals(EmailAddress) && ValidationHelper.IsEmail(UserName))
                {
                    yield return new ValidationResult("Username cannot be an email address unless it's the same as your email address!");
                }
            }
        }
    }
}
