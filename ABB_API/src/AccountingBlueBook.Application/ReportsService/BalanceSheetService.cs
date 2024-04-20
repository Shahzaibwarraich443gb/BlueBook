using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities.Reports;
using AccountingBlueBook.Entities.MainEntities.Reports.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.ReportsService
{
    [AbpAuthorize]
    public class BalanceSheetService : AccountingBlueBookAppServiceBase
    {
        private readonly IReportRepository _reportRepository;
        private readonly IRepository<Company> _companyRepository;

        public BalanceSheetService(IReportRepository reportRepository,
            IRepository<Company> companyRepository)
        {
            _reportRepository = reportRepository;
            _companyRepository = companyRepository;
        }

        public async Task<List<BalanceSheetDto>> GetList(DateTime _StartDate, DateTime _EndDate)
        {
            try
            {
                var orderedList = new List<BalanceSheetDto>();
                if (AbpSession.TenantId.HasValue)
                {
                    var res = await _reportRepository.GetBalanceSheet(_StartDate, _EndDate, (int)AbpSession.TenantId);
                    return orderedList;
                }
                return orderedList;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred while getting balanceSheet", ex.Message);
            }
        }
    }
}
