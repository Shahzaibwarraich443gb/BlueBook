using Abp.Application.Services.Dto;
using System;

namespace AccountingBlueBook.ChartOfAccounts
{
    //custom PagedResultRequestDto
    public class PagedChartOfAccountResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool IsActive { get; set; }
    }
}
