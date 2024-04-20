using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountingBlueBook.GeneralLedgers
{
    public interface IGeneralLedgerAppService
    {
        Task AddLedger(AddGeneralLedgarInputDto input);
        Task<List<GeneralLedgerOutputDto>> GetLedgerForTable(GetGeneralLedgerInputDto input);
        Task SaveHeaders(string Headers, int CustomerId, string Type);
        Task<List<GeneralLedgerChartOfAccountData>> CoaData(List<ChartOfAccount> CoaList, List<InvoiceDetail> InvoiceDetailList, List<ProductService> ProductList);
        Task<LedgerHeaders> GetHeaders(int CustomerId, string Type);
    }
}
