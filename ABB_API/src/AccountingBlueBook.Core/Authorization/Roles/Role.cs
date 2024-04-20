using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using AccountingBlueBook.Authorization.Users;

namespace AccountingBlueBook.Authorization.Roles
{
    public class Role : AbpRole<User>
    {
        public const int MaxDescriptionLength = 5000;

        public Role()
        {
        }

        public Role(int? tenantId, string displayName)
            : base(tenantId, displayName)
        {
        }

        public Role(int? tenantId, string name, string displayName)
            : base(tenantId, name, displayName)
        {
        }

        [StringLength(MaxDescriptionLength)]
        public string Description {get; set;}
        // add three new columns
        public bool IpRestriction { get; set; }
        public string IpAddress { get; set; }
        public bool IsActive { get; set; }
    }
}
