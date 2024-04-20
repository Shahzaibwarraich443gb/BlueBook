using Abp.Application.Services.Dto;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.ProductCategories.Dto
{
    public  class CreateOrEditProductCategoryDto : EntityDto
    {
        public string Name { get; set; }
        public string Nature { get; set; }
        public ProductCategoryEnum ProductCategoryEnum { get; set; }
        public bool IsActive { get; set; }
        public int? CompanyId { get; set; }
    }
}
