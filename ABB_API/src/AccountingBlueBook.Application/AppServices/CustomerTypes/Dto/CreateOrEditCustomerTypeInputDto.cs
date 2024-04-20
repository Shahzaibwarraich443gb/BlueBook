using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.CustomerTypes.Dto
{
    public  class CreateOrEditCustomerTypeInputDto : EntityDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
