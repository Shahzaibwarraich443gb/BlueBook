using Abp.Application.Services.Dto;

namespace AccountingBlueBook.AppServices.usersGroup.Dto
{
    public class CreateOrEditGeneralUserGroupInputDto :  EntityDto
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
        public bool IpRestrictionUserGroup { get; set; }

        public string IpAddress { get; set; }
        public bool IsActive { get; set; }

         }
    }
