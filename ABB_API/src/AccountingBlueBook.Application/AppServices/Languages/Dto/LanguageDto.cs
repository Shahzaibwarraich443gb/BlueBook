using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Languages.Dto
{
    public class LanguageDto : EntityDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? CompanyId { get; set; }

        public bool IsActive { get; set; }
        public string CompanyName { get; set; }
    }
}
