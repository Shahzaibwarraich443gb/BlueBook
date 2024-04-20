using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.CorporateTaxes
{
    public interface ICorporateTaxAppService
    {
        Task<CorporateTaxDto> CorporateTaxGet(CorporateTaxDto input);
        Task SaveCorporateTax(CorporateTaxDto input);
    }
}
