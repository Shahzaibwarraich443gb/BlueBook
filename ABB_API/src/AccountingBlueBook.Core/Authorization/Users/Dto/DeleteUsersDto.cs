using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Authorization.Users.Dto
{
    public class DeleteUsersDto
    {
        public long UserId { get; set; }
        public long TenantId { get; set; }
        public DateTime DeleteionTime { get; set; }
    }
}
