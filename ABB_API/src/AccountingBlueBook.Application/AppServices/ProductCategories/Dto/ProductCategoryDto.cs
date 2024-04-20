using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.ProductCategories.Dto
{
    public  class ProductCategoryDto : EntityDto
    {
        public string Name { get; set; }

        public string Nature { get; set; }
        public ProductCategoryEnum ProductCategoryEnum { get; set; }
        public bool IsActive { get; set; }
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
