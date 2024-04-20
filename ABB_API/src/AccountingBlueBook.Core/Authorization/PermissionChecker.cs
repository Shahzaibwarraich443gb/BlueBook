using Abp.Authorization;
using AccountingBlueBook.Authorization.Roles;
using AccountingBlueBook.Authorization.Users;

namespace AccountingBlueBook.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
