using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using AccountingBlueBook.Entities;
using AccountingBlueBook.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.CorporateTaxes
{
    public class CorporateTaxAppService : AccountingBlueBookAppServiceBase, ICorporateTaxAppService
    {
        private readonly IRepository<CorporateTax, long> corporateTaxRepository;

        public CorporateTaxAppService(IRepository<CorporateTax, long> corporateTaxRepository)
        {
            this.corporateTaxRepository = corporateTaxRepository;
        }
        public async Task SaveCorporateTax(CorporateTaxDto input)
        {
            var corporateTax = await corporateTaxRepository.FirstOrDefaultAsync(x => x.CustomerId == input.CustomerId && x.FinancialYear == input.FinancialYear);
            if (corporateTax != null)
            {
                corporateTax.Tenure = input.Tenure;
                corporateTax.FinancialYear = input.FinancialYear;
                corporateTax.CustomerId = input.CustomerId;
                corporateTax.LegalStatus = input.LegalStatus;
                corporateTax.MonthlyData = input.monthlyData;
                corporateTax.OtherExpense = input.OtherExpense;
                corporateTax.OtherIncome = input.OtherIncome;
                corporateTax.CostOfSale = input.CostOfSale;
                await corporateTaxRepository.UpdateAsync(corporateTax);
            }
            else
            {
                var corpObj = ObjectMapper.Map<CorporateTax>(input);
                await corporateTaxRepository.InsertAsync(corpObj);
            }
        }

        private string GetDisplayName(Enum value)
        {
            return value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(false)
                .OfType<DisplayAttribute>()
                .FirstOrDefault()
                ?.Name ?? value.ToString();
        }

        public async Task<CorporateTaxDto> CorporateTaxGet(CorporateTaxDto input)
        {
            var corporateTax = await corporateTaxRepository.FirstOrDefaultAsync(x => x.CustomerId == input.CustomerId && x.FinancialYear == input.FinancialYear);

            if (corporateTax == null)
            {
                corporateTax = new CorporateTax();
            }

            if (corporateTax.OtherIncome == null)
            {
                corporateTax.OtherIncome = JsonConvert.SerializeObject(Enum.GetValues(typeof(OtherIncomeDefaultEnum))
                    .Cast<OtherIncomeDefaultEnum>()
                    .Select(e => new { Value = 0, Name = GetDisplayName(e) })
                    .ToList());
            }

            if (corporateTax.CostOfSale == null)
            {
                corporateTax.CostOfSale = JsonConvert.SerializeObject(Enum.GetValues(typeof(CostOfSaleDefaultEnum))
                    .Cast<CostOfSaleDefaultEnum>()
                    .Select(e => new { Value = 0, Name = GetDisplayName(e) })
                    .ToList());
            }

            if (corporateTax.OtherExpense == null)
            {
                corporateTax.OtherExpense = JsonConvert.SerializeObject(Enum.GetValues(typeof(OtherExpenseDefaultEnum))
                    .Cast<OtherExpenseDefaultEnum>()
                    .Select(e => new { Value = 0, Name = GetDisplayName(e) })
                    .ToList());
            }

            return ObjectMapper.Map<CorporateTaxDto>(corporateTax);
        }


    }
}
