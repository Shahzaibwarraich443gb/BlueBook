using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.ProductCategories.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.ProductCategories
{
    public  interface IProductCategoryAppService      {

        Task<List<ProductCategoryDto>> GetAll();
        Task CreateOrEdit(CreateOrEditProductCategoryDto input);
        Task<ProductCategoryDto> Get(EntityDto input);
        Task Delete(EntityDto input);

    }
}
