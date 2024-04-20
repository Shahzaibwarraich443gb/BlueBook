using Abp.Application.Services.Dto;

namespace AccountingBlueBook.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

