using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;

namespace AccountingBlueBook.AppServices.ProductCategories.Dto
{
    public  class ProductCategoryMapProfile : Profile
    {
        public ProductCategoryMapProfile()
        {
            CreateMap<ProductCategoryDto, ProductCategory>();
            CreateMap<CreateOrEditProductCategoryDto, ProductCategory>();
        }
    }
}

