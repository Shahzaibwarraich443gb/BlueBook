using AccountingBlueBook.Authorization.Users.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Authorization.Users
{
    public interface IEmailAppServices
    {
        Task SendMail(EmailsDto input);
    }
}
