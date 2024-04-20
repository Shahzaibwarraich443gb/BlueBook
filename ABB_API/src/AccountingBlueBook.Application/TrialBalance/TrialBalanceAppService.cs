using Abp.Domain.Repositories;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.TrialBalance
{
    public class TrialBalanceAppService : AccountingBlueBookAppServiceBase
    {
        private readonly IRepository<GeneralLedger, long> _GeneralLedgerAppService;
        private readonly IRepository<ChartOfAccount> _ChartOfAccountAppService;
        private readonly IRepository<Company> _CompanyRepository;

        public TrialBalanceAppService(IRepository<GeneralLedger, long> generalLedgerAppService,
                                      IRepository<ChartOfAccount> chartOfAccountAppService,
                                      IRepository<Company> companyRepository)
        {
            _GeneralLedgerAppService = generalLedgerAppService;
            _ChartOfAccountAppService = chartOfAccountAppService;
            _CompanyRepository = companyRepository;
        }

        [HttpPost]
        public async Task<List<TrialBalanceDto>> GetTrialBalance(TrialBalanceInputDto Input)
        {
            int CompanyId = _CompanyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;

            List<ChartOfAccount> ChartOfAccountList = await _ChartOfAccountAppService.GetAll().Where(x => x.CompanyId == CompanyId).Include(x => x.MainHead).ToListAsync();

            List<MainHead> MainHeadList = ChartOfAccountList.Select(x => x.MainHead).Distinct().ToList();

            List<GeneralLedger> GeneralLedgerList = await _GeneralLedgerAppService.GetAll().Where(x => x.CreationTime.Date >= DateTime.Now.AddDays(-1).Date).Include(x => x.GeneralLedgerDetails).ToListAsync();

            List<TrialBalanceDto> TrialBalanceList = new();

            foreach (MainHead Data in MainHeadList)
            {

                TrialBalanceList.Add(new TrialBalanceDto
                {
                    Type = "Header",
                    MainHeadId = Data.Id,
                    MainHeadName = Data.Name
                });

                List<TrialBalanceDto> SubHeadList = ChartOfAccountList.Where(x => x.MainHeadId == Data.Id).Select(x =>
                {
                    double CurrentCreditAmount = GeneralLedgerList.Where(y => y.GeneralLedgerDetails.Any(z => z.ChartOfAccountId == x.Id) && y.CreationTime.Date >= Input.StartDate.Date && y.CreationTime.Date <= Input.EndDate.Date).Sum(y => y.GeneralLedgerDetails.Sum(z => z.CreditAmount));
                    double CurrentDebitAmount = GeneralLedgerList.Where(y => y.GeneralLedgerDetails.Any(z => z.ChartOfAccountId == x.Id) && y.CreationTime.Date >= Input.StartDate.Date && y.CreationTime.Date <= Input.EndDate.Date).Sum(y => y.GeneralLedgerDetails.Sum(z => z.DebitAmount));
                    double PrevDayCreditAmount = GeneralLedgerList.Where(y => y.GeneralLedgerDetails.Any(z => z.ChartOfAccountId == x.Id) && y.CreationTime.Date >= Input.StartDate.AddDays(-1).Date && y.CreationTime.Date <= Input.EndDate.AddDays(-1).Date).Sum(y => y.GeneralLedgerDetails.Sum(z => z.CreditAmount));
                    double PrevDayDebitAmount = GeneralLedgerList.Where(y => y.GeneralLedgerDetails.Any(z => z.ChartOfAccountId == x.Id) && y.CreationTime.Date >= Input.StartDate.AddDays(-1).Date && y.CreationTime.Date <= Input.EndDate.AddDays(-1).Date).Sum(y => y.GeneralLedgerDetails.Sum(z => z.DebitAmount));
                    return new TrialBalanceDto()
                    {
                        MainHeadId = Data.Id,
                        SubHeadId = x.Id,
                        SubHeadName = x.AccountDescription,
                        CreditAmount = CurrentCreditAmount,
                        DebitAmount = CurrentDebitAmount,
                        Balance = Math.Round((PrevDayDebitAmount - PrevDayCreditAmount) + (CurrentDebitAmount - CurrentCreditAmount), 2),
                        OpeningBalance = Math.Round(PrevDayDebitAmount - PrevDayCreditAmount, 2),
                        Type = "Data"
                    };
                }).OrderByDescending(x => x.SubHeadId).ToList();

                TrialBalanceList.AddRange(SubHeadList);

                TrialBalanceList.Add(new TrialBalanceDto
                {
                    Type = "Sum",
                    MainHeadId = Data.Id,
                    MainHeadName = Data.Name,
                    TotalCreditAmount = Math.Round(TrialBalanceList.Where(x => x.MainHeadId == Data.Id).Sum(x => x.CreditAmount),2),
                    TotalDebitAmount = Math.Round(TrialBalanceList.Where(x => x.MainHeadId == Data.Id).Sum(x => x.DebitAmount), 2)
                });
            }

            return TrialBalanceList;

        }
    }
}
