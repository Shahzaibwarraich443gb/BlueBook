using System.Collections.Generic;

namespace AccountingBlueBook.Roles.Dto
{
    public class GetRoleForEditOutput
    {
        public RoleEditDto newRole { get; set; }
        public RoleDto Role { get; set; }

        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}