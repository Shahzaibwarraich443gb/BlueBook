using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Emails.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Emails
{
    public interface IEmailAppService
    {
        Task<List<EmailDto>> GetAll();
        Task CreateOrEdit(CreateOrEditEmailInputDto input);
        Task<EmailDto> Get(EntityDto input);
        Task Delete(EntityDto input);
    }
}
