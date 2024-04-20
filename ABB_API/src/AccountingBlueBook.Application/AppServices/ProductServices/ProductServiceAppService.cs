using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using AccountingBlueBook.AppServices.Languages.Dto;
using AccountingBlueBook.AppServices.Languages;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.ObjectMapping;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.AppServices.ProductServices.Dto;
using Microsoft.EntityFrameworkCore;
using AccountingBlueBook.ChartOfAccounts.Dto;

namespace AccountingBlueBook.AppServices.ProductServices
{
    public  class ProductServiceAppService : AccountingBlueBookAppServiceBase, IProductServiceAppService
    {
        private readonly IRepository<ProductService> _productServiceRepository;
        private readonly IRepository<ChartOfAccount> _chartOfAccountRepository;
        private IObjectMapper ObjectMapper;

        public ProductServiceAppService(IRepository<ChartOfAccount> chartOfAccountRepository,  IRepository<ProductService> productServiceRepository, IObjectMapper objectMapper)
        {
            _productServiceRepository = productServiceRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEdit(CreateOrEditProductServiceInputDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Cearte(input);
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditProductServiceInputDto input)
        {
            var productService = await _productServiceRepository.FirstOrDefaultAsync((int)input.Id); 
            if(productService.Id > 0)
            {
                // For Update               
                productService.Name = input.Name;
                productService.IsActive = input.IsActive;
                productService.AutomaticExpense = input.AutomaticExpense;
                productService.SKU = input.SKU;
                productService.CostPrice = input.CostPrice;
                productService.SalePrice = input.SalePrice;
                productService.ProductCategoryId = input.ProductCategoryId;
                productService.VendorId = input.VendorId;
                productService.SaleInformation = input.SaleInformation;
                productService.TypeId = input.TypeId;
                productService.SaleInformation = input.SaleInformation;
                productService.IncomeAccountId = input.IncomeAccountId;
                productService.ExpenseAccountId = input.ExpenseAccountId;
                productService.SaleTax = input.SaleTax;
                productService.IsActive = false;
                productService.ToCostPrice = DateTime.Now;
                productService.ToSalePrice = DateTime.Now;
                productService.AdvanceSaleTaxAccountId = input.AdvanceSaleTaxAccountId;
                productService.LiabilityAccountId = input.LiabilityAccountId;
                await _productServiceRepository.UpdateAsync(productService);
                // for Save 
                //var productServiceForSave = ObjectMapper.Map<ProductService>(input);
                //productServiceForSave.FromCostPrice = DateTime.Now;
                //productServiceForSave.FromSalePrice = DateTime.Now;
                //productServiceForSave.ToSalePrice = null;
                //productServiceForSave.ToCostPrice = null;
                //productServiceForSave.IsActive = true;
                //await _productServiceRepository.InsertAsync(productServiceForSave);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        [UnitOfWork]
        private async Task Cearte(CreateOrEditProductServiceInputDto input)
        {
            var productService = ObjectMapper.Map<ProductService>(input);
            productService.FromCostPrice = DateTime.Now;
            productService.FromSalePrice = DateTime.Now;
            productService.ToSalePrice = null;
            productService.ToCostPrice = null;
            productService.IsActive = true;
            await _productServiceRepository.InsertAsync(productService);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(EntityDto input)
        {
            var language = await _productServiceRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            await _productServiceRepository.DeleteAsync(language);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<ProductServiceDto> Get(EntityDto input)
        {
            var productService = await _productServiceRepository.GetAll() .FirstOrDefaultAsync(x => x.Id == input.Id);
            if (productService != null)
            {
                var productServiceDto = new ProductServiceDto();
                productServiceDto.Id = productService.Id; 
                productServiceDto.Name = productService.Name;
                productServiceDto.IsActive = productService.IsActive;
                productServiceDto.SalePrice= productService.SalePrice; 
                productServiceDto.AutomaticExpense = productService.AutomaticExpense;
                productServiceDto.SKU = productService.SKU;
                productServiceDto.CostPrice = productService.CostPrice;
                productServiceDto.SaleTax = productService.SaleTax;
                productServiceDto.ProductCategoryId = productService.ProductCategoryId;
                productServiceDto.VendorId = productService.VendorId;
                productServiceDto.SaleInformation = productService.SaleInformation;
                productServiceDto.TypeId = productService.TypeId;
                productServiceDto.SaleInformation = productService.SaleInformation;
                productServiceDto.IncomeAccountId = productService.IncomeAccountId;
                productServiceDto.ExpenseAccountId = productService.ExpenseAccountId;
                productServiceDto.LiabilityAccountId = productService.LiabilityAccountId;
                productServiceDto.AdvanceSaleTaxAccountId = productService.AdvanceSaleTaxAccountId;
                return productServiceDto;
            }
            return null;

        }

        public async Task<List<ProductServiceDto>> GetAll()
        { 
            var filteredQuery = _productServiceRepository.GetAll().Include(data=>data.ProductCategory).AsQueryable();


            var data = from o in filteredQuery
                       select new ProductServiceDto
                       {
                           Id = o.Id,
                           IsActive = o.IsActive,
                           Name = o.Name,
                           ProductCaterogyName = o.ProductCategory.Name,
                           SaleTax = o.SaleTax,
                           SalePrice = o.SalePrice,
                           CostPrice = o.CostPrice,
                           FromSalePrice = o.FromSalePrice,
                           ToSalePrice = o.ToSalePrice,
                           FromCostPrice = o.FromCostPrice,
                           ToCostPrice = o.ToCostPrice, 
                           AutomaticExpense = o.AutomaticExpense
                       };
            return await data.ToListAsync();
        }

        public async Task<List<ProductServiceDto>> GetAllIncome()
        {
            var filteredQuery = _productServiceRepository.GetAll().Where(x => x.IncomeAccountId != null).Include(data => data.ProductCategory).AsQueryable();


            var data = from o in filteredQuery
                       select new ProductServiceDto
                       {
                           Id = o.Id,
                           IsActive = o.IsActive,
                           Name = o.Name,
                           ProductCaterogyName = o.ProductCategory.Name,
                           SaleTax = o.SaleTax,
                           SalePrice = o.SalePrice,
                           CostPrice = o.CostPrice,
                           FromSalePrice = o.FromSalePrice,
                           ToSalePrice = o.ToSalePrice,
                           FromCostPrice = o.FromCostPrice,
                           ToCostPrice = o.ToCostPrice,
                           AutomaticExpense = o.AutomaticExpense

                       };
            return await data.ToListAsync();
        }
        
        
        public async Task<List<ProductServiceDto>> GetAllExpense()
        {
            var filteredQuery = _productServiceRepository.GetAll().Where(x => x.ExpenseAccountId != null).Include(data => data.ProductCategory).AsQueryable();


            var data = from o in filteredQuery
                       select new ProductServiceDto
                       {
                           Id = o.Id,
                           IsActive = o.IsActive,
                           Name = o.Name,
                           ProductCaterogyName = o.ProductCategory.Name,
                           SaleTax = o.SaleTax,
                           SalePrice = o.SalePrice,
                           CostPrice = o.CostPrice,
                           FromSalePrice = o.FromSalePrice,
                           ToSalePrice = o.ToSalePrice,
                           FromCostPrice = o.FromCostPrice,
                           ToCostPrice = o.ToCostPrice,
                           AutomaticExpense = o.AutomaticExpense
                       };
            return await data.ToListAsync();
        }

        public  async Task<List<ChartOfAccountDto>> GetAllChartAccountIncome()
        {
            var filteredQuery = _chartOfAccountRepository.GetAll().Where(data=>(int)data.AccountNature == 3);

            var data = from o in filteredQuery
                       select new ChartOfAccountDto
                       {
                           Id = o.Id,
                           AccountType = o.AccountType == null ? null : o.AccountType.Name,
                           AccountDescription = o.AccountDescription,
                           AccountTypeId = o.AccountTypeId,
                           MainHeadId = o.MainHeadId,
                           IsActive = o.IsActive,
                           AccountNature = (int)o.AccountNature == 3 ?  "Income" : null,
                           MainHead = o.MainHead == null ? null : o.MainHead.Name
                       };

            return await data.ToListAsync();
        }

        public async Task<List<ChartOfAccountDto>> GetAllChartAccountExpense()
        {
            var filteredQuery = _chartOfAccountRepository.GetAll().Where(data => (int)data.AccountNature == 4);
            var data = from o in filteredQuery
                       select new ChartOfAccountDto
                       {
                           Id = o.Id,
                           AccountType = o.AccountType == null ? null : o.AccountType.Name,
                           AccountDescription = o.AccountDescription,
                           AccountTypeId = o.AccountTypeId,
                           MainHeadId = o.MainHeadId,
                           IsActive = o.IsActive,
                           AccountNature = (int)o.AccountNature == 4 ? "Expense" : null,
                           MainHead = o.MainHead == null ? null : o.MainHead.Name
                       };

            return await data.ToListAsync();
        }
    }
}
