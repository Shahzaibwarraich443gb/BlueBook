using AccountingBlueBook.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Roles.Dto
{
    public class FlatPermissionWithLevelDto : FlatPermissionDto
    {
        public int Level { get; set; }
    }
}
