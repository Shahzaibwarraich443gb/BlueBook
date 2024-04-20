using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Languages.Dto
{
    public class CreateOrEditLanguageInputDto : EntityDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public bool IsActive { get; set; }
        public int? CompanyId { get; set; }
    }
}
