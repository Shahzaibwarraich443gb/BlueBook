using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using AccountingBlueBook.Authorization.Roles;

namespace AccountingBlueBook.Roles.Dto
{
    public class RoleEditDto: EntityDto<int>
    {
        [Required]
        [StringLength(AbpRoleBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpRoleBase.MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        [StringLength(Role.MaxDescriptionLength)]
        public string Description { get; set; }

        public bool IsStatic { get; set; }
        // add three new columns
        public bool IpRestriction { get; set; }
        public List<string> IpAddress { get; set; } = new List<string>();
       // public string IpAddress { get; set; }
        public bool IsActive { get; set; }
    }
}