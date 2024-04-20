using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities.Reports;
using AccountingBlueBook.Entities.MainEntities.Reports.Dto;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.ReportsService
{
    [AbpAuthorize]
    public class EmployeActivitiesAppService : AccountingBlueBookAppServiceBase
    {
        private readonly IReportRepository _reportRepository;
        private readonly IRepository<Company> _companyRepository;

        public EmployeActivitiesAppService(IReportRepository reportRepository,
            IRepository<Company> companyRepository)
        {
            _reportRepository = reportRepository;
            _companyRepository = companyRepository;
        }

        public async Task<List<AuditLogsDto>> GetList(DateTime _StartDate, DateTime _EndDate, string MethodName)
        {
            try
            {
                var tenatid = AbpSession.TenantId == null ? 1 : (int)AbpSession.TenantId;
                var res = await _reportRepository.GetAllAuditlogs(_StartDate, _EndDate, tenatid, MethodName);
                return res;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred while getting EmployeeActivities", ex.Message);
            }
        }
    }
}
