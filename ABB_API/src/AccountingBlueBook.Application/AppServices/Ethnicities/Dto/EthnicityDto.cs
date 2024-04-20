using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Ethnicities.Dto
{
    public class EthnicityDto : EntityDto
    {
        public string Name { get; set; }
        public string Descripition { get; set; }
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
        public bool ? IsActive { get; set; }
    }
}
