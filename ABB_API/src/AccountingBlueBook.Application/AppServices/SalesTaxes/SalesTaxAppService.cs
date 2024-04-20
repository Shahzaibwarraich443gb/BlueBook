using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using AccountingBlueBook.Entities.MainEntities;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.SalesTaxes
{
    public class SalesTaxAppService : AccountingBlueBookAppServiceBase, ISalesTaxAppService
    {
        private readonly IRepository<SalesTax, long> _salesTaxRepository;

        public SalesTaxAppService(IRepository<SalesTax, long> salesTaxRepository)
        {
            _salesTaxRepository = salesTaxRepository;
        }
        public async Task<SalesTaxDto> SalesTaxGet(SalesTaxDto input)
        {
            var salesTax = await _salesTaxRepository.FirstOrDefaultAsync(x => x.CustomerId == input.CustomerId && x.FinancialYear == input.FinancialYear);
            SalesTaxDto salesTaxDto = new SalesTaxDto();
            if(salesTax == null)
            {
                return salesTaxDto;
            }
            salesTaxDto.CustomerId = salesTax.CustomerId;
            salesTaxDto.FinancialYear = salesTax.FinancialYear;
            salesTaxDto.LegalStatus = salesTax.LegalStatus;
            salesTaxDto.NonTaxableAmount = salesTax.NonTaxableAmount;
            salesTaxDto.SalesRatePercentage = salesTax.SalesRatePercentage;
            salesTaxDto.TaxDataMonthly = salesTax.TaxDataMonthly;
            salesTaxDto.TenureForm = salesTax.TenureForm;
            salesTaxDto.SalesTaxAmount = salesTax.SalesTaxAmount;
            salesTaxDto.TaxableSales = salesTax.TaxableSales;
            salesTaxDto.Id = salesTax.Id;
            return salesTaxDto;
        }

        public async Task SaveSalesTax(SalesTaxDto input)
        {
           var salesTax = await _salesTaxRepository.FirstOrDefaultAsync(x => x.CustomerId == input.CustomerId && x.FinancialYear == input.FinancialYear);
            if(salesTax == null)
            {
                salesTax = new SalesTax();
            }
            salesTax.CustomerId = input.CustomerId;
            salesTax.FinancialYear = input.FinancialYear;
            salesTax.LegalStatus = input.LegalStatus;
            salesTax.NonTaxableAmount = input.NonTaxableAmount;
            salesTax.SalesRatePercentage = input.SalesRatePercentage;
            salesTax.TaxDataMonthly = input.TaxDataMonthly;
            salesTax.TenantId = (int)AbpSession.TenantId;
            salesTax.TenureForm = input.TenureForm;
            salesTax.SalesTaxAmount = input.SalesTaxAmount;
            salesTax.TaxableSales = input.TaxableSales;
            if (salesTax.Id > 0)
            {
                 _salesTaxRepository.Update(salesTax);
            }
            else
            {
                await _salesTaxRepository.InsertAsync(salesTax);
            }
            await CurrentUnitOfWork.SaveChangesAsync();

        }

    }
}
