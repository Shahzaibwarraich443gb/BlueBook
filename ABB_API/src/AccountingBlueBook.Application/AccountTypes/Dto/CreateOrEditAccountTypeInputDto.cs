using Abp.Application.Services.Dto;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AccountTypes.Dto
{
    public class CreateOrEditAccountTypeInputDto : EntityDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int AccountNature { get; set; }
    }
}
