using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Authorization.Users;
using Abp.Extensions;

namespace AccountingBlueBook.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "123qwe";

    
        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }
        public bool IsAdmin { get; set; }
        public long? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public string EmailAddress { get; set; }

        public long? EmployeeId { get; set; }
        public DateTime? TokenExpiaryTime { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastOnlineTime { get; set; }

        //public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        //{
        //    var user = new User
        //    {
        //        TenantId = tenantId,

        //        UserName = AdminUserName,
        //        Name = AdminUserName,
        //        Surname = AdminUserName,
        //        EmailAddress = emailAddress,
        //        Roles = new List<UserRole>()
        //    };

        //    user.SetNormalizedNames();

        //    return user;
        //}
        public static User CreateTenantAdminUser(int tenantId, string emailAddress, string userName = "", string name = "")
        {
            var user = new User
            {

                TenantId = tenantId,
                UserName = userName.Length > 0 ? userName : AdminUserName,
                Name = name,
                Surname = "",
                EmailAddress = emailAddress,
                Roles = new List<UserRole>(),
                IsAdmin = true,
                LastOnlineTime = DateTime.Now,
               

            };
       
           
            user.SetNormalizedNames();

            return user;
        }
    }
}
