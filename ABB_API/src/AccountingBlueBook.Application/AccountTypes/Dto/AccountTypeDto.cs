using Abp.Application.Services.Dto;
using AccountingBlueBook.Enums;

namespace AccountingBlueBook.AccountTypes.Dto
{
    public class AccountTypeDto : EntityDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public AccountNature AccountNature { get; set; }
    }
}
