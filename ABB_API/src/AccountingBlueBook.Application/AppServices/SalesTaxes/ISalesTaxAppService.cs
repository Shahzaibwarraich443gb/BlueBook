using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.SalesTaxes
{
    public interface ISalesTaxAppService
    {
        Task<SalesTaxDto> SalesTaxGet( SalesTaxDto input);

        Task SaveSalesTax(SalesTaxDto input);
    }
}
