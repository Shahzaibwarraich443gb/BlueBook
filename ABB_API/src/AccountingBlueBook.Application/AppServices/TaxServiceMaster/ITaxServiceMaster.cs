using Abp.Application.Services.Dto;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.TaxServiceMaster
{
    public interface ITaxServiceMaster
    {
        Task<List<TaxFillingStatusDto>> GetTaxFillingStatus();
        Task<List<TenureDto>> GetTenure();
        Task<List<FormEnum>> GetFormList();

        Task<List<string>> GetIncomeDescriptions();
    }
}
