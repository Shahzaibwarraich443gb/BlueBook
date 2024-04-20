using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using AccountingBlueBook.AppServices.ProductCategories.Dto;
using AccountingBlueBook.Entities.MainEntities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.ProductCategories
{
    public  class ProductCategoryAppService : AccountingBlueBookAppServiceBase, IProductCategoryAppService
    {
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private IObjectMapper ObjectMapper;

        public ProductCategoryAppService(IRepository<ProductCategory> productCategoryRepository, IObjectMapper objectMapper)
        {
            _productCategoryRepository = productCategoryRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEdit(CreateOrEditProductCategoryDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Create(input);
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditProductCategoryDto input)
        {
            //todo:  CompanyId 
            var productCategory = await _productCategoryRepository.FirstOrDefaultAsync((int)input.Id);
            productCategory.Name = input.Name;
            productCategory.IsActive  = input.IsActive;
            productCategory.Nature = input.Nature;
            productCategory.ProductCategoryEnum = input.ProductCategoryEnum;
            //productCategory.CompanyId = input.CompanyId;

            await _productCategoryRepository.UpdateAsync(productCategory);
        }

        [UnitOfWork]
        private async Task Create(CreateOrEditProductCategoryDto input)
        {
            //todo:  CompanyId 
           
            var productCategory = ObjectMapper.Map<ProductCategory>(input);
            productCategory.IsActive = true;
            await _productCategoryRepository.InsertAsync(productCategory);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(EntityDto input)
        {
            var productCategory = await _productCategoryRepository.FirstOrDefaultAsync((int)input.Id);
            await _productCategoryRepository.DeleteAsync(productCategory);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task<ProductCategoryDto> Get(EntityDto input)
        {
            var productCategory = await _productCategoryRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            if (productCategory != null)
            {
                var obj = new ProductCategoryDto();
                obj.Id = productCategory.Id;
                obj.ProductCategoryEnum = productCategory.ProductCategoryEnum;
                obj.IsActive = productCategory.IsActive;
                obj.Nature = productCategory.Nature;
                obj.Name = productCategory.Name;
                obj.IsActive =   productCategory.IsActive;
                return obj;
            }
            return null;
        }

        public async Task<List<ProductCategoryDto>> GetAll()
        {
            //todo:  CompanyId 
            var filteredQuery = _productCategoryRepository.GetAll().Include(data=>data.Company).AsQueryable();

            var data = from o in filteredQuery
                       select new ProductCategoryDto
                       {
                           Id = o.Id,
                           Name = o.Name,
                           IsActive = o.IsActive,
                           Nature = o.Nature,
                           Company = o.Company == null ? null : o.Company,
                           ProductCategoryEnum = o.ProductCategoryEnum,
                           
                       };
            return await data.ToListAsync();
        }

    }
}
