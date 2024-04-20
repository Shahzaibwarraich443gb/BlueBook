using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.MainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.ProductServices.Dto
{
    public class CreateOrEditProductServiceInputDto : EntityDto
    {
        public string Name { get; set; }
        public int? IncomeAccountId { get; set; }
        public DateTime? FromSalePrice { get; set; }
        public DateTime? ToSalePrice { get; set; }
        public DateTime? FromCostPrice { get; set; }
        public DateTime? ToCostPrice { get; set; }
        public int? ExpenseAccountId { get; set; }
        public int? TypeId { get; set; }
        public decimal SalePrice { get; set; }
        public string SaleTax { get; set; }
        public string ExpenseSaleTax { get; set; }
        public decimal CostPrice { get; set; }
        public string SaleInformation { get; set; }
        public bool? AutomaticExpense { get; set; }
        public string SKU { get; set; }
        public bool IsActive { get; set; }
        public int? VendorId { get; set; }
        public int? AdvanceSaleTaxAccountId { get; set; }
        public int? LiabilityAccountId { get; set; }

        public int? ProductCategoryId { get; set; } 
    }
}
