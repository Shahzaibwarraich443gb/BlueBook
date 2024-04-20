
using Abp.Domain.Repositories;
using AccountingBlueBook.Entities.MainEntities.Customers.Dto;
using AccountingBlueBook.Entities.MainEntities.Reports.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountingBlueBook.Entities.MainEntities.Reports
{
    public interface IReportRepository : IRepository
    {
        Task<List<DailyReceiptDto>> GetAllDailyRecepit(DateTime? _StartDate, DateTime? _EndDate,long? _PaymentMethodId, long? _AccountId, int CompanyID);       
        Task<List<AuditLogsDto>> GetAllAuditlogs(DateTime startDate, DateTime endDate, int tenatid, string MethodName);

        Task<List<SourceReferalDto>> GetAllSourceReferal(DateTime startDate, DateTime endDate,  long SoureceReferalId);
        Task<List<BalanceSheetDto>> GetBalanceSheet(DateTime startDate, DateTime endDate, int tenantId);


    }
}
