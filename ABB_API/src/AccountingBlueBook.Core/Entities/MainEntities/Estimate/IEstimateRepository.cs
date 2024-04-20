using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities.Estimate
{
    public interface IRepository : IRepository<Estimates, long>
    {
        Task<string> GetVoucherNumber(string voucherTypeCode, int tenantId);
        Task<string> GetInvoiceBalance(long id);
        Task<string> GetCustomerBalance(long customerId);
        Task<string> GetInvoiceNo(int tenantId, int invoiceType);

        Task<List<CustomerTransaction>> GetCustomerTransaction(EntityDto input);
    }
}
