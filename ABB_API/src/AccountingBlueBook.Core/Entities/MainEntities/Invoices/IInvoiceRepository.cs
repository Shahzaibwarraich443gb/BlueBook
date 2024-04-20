using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using AccountingBlueBook.AppServices.Invoices.dto;
using AccountingBlueBook.AppServices.JournalVoucher.Dto;
using AccountingBlueBook.Entities.MainEntities.Customers.Dto;
using AccountingBlueBook.Entities.MainEntities.Vouchers;
using AccountingBlueBook.Entities.NormalizeEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities.Invoices
{
    public interface IInvoiceRepository : IRepository<Invoice, long>
    {
        Task<string> GetVoucherNumber(string voucherTypeCode, int tenantId);
        Task<string> GetInvoiceBalance(long id);
        Task<string> GetCustomerBalance(long customerId);
        Task<string> GetInvoiceNo(int tenantId, int invoiceType);
        Task<List<ReceviedPayment>> GetReceivedPaymentList(int tenantId,EntityDto input);
        Task<List<ReceviedPayment>> GetPurchasePaymentList(int tenantId, EntityDto input);
        Task<List<invoiceDetails>> GetInvoiceDetails(int tenantId, EntityDto input);
        Task<List<PrintDetail>> GetPrintDetails(int tenantId, long invoicecId);
        Task<List<CustomerTransactionDto>> GetCustomerTransaction(int customerId, int tenantId);
        Task<List<CustomerTransactionDto>> GetAllTransactions(int tenantId);
        Task<List<ReceviedPayment>> GetReceivedPaymentDetails(int tenantId, EntityDto input);
        Task<List<VoucherList>> GetAllVouchers(int tenantId);
    }
}
