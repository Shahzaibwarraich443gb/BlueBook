using Abp.Application.Services.Dto;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.MainHeading.Dto
{
    public class CreateOrEditMainHeadingInputDto : EntityDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int? AccountTypeId { get; set; }
    }
}
