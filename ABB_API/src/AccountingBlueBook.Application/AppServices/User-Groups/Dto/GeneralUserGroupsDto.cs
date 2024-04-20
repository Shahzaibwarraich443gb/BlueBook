using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.usersGroup.Dto
{
    public  class GeneralUsersGroupDto : EntityDto
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
        public bool IpRestrictionUserGroup { get; set; }
        public string IpAddress { get; set; }
        public bool IsActive { get; set; }
        
    }
}
