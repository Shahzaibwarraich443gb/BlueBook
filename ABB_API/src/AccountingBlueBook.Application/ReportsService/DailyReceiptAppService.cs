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
    public class DailyReceiptAppService : AccountingBlueBookAppServiceBase
    {
        private readonly IReportRepository _reportRepository;
        private readonly IRepository<Company> _companyRepository;

        public DailyReceiptAppService(IReportRepository reportRepository,
            IRepository<Company> companyRepository)
        {
            _reportRepository = reportRepository;
            _companyRepository = companyRepository;
        }

        public async Task<List<DailyReceiptDto>> GetList(DateTime? _StartDate, DateTime? _EndDate, long? _PaymentMethodId, long? _AccountId)
        {
            try
            {
                var company = _companyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;
                var res = await _reportRepository.GetAllDailyRecepit(_StartDate, _EndDate, _PaymentMethodId, _AccountId, company);

                var paymentMethods = res.Select(x => x.PaymentMethod).Distinct();

                var orderedList = new List<DailyReceiptDto>();

                foreach (var paymentMethod in paymentMethods)
                {
                    var paymentMethodItems = res.Where(x => x.PaymentMethod == paymentMethod).ToList();

                    var dtoIndex = new DailyReceiptDto();
                    dtoIndex.PaymentMethod = $"{paymentMethod} Receipt";
                    var firstIndex = paymentMethodItems.FindIndex(x => x.PaymentMethod == paymentMethod);
                    if (firstIndex != -1)
                    {
                        paymentMethodItems.Insert(firstIndex, dtoIndex);
                    }

                    var total = paymentMethodItems.Sum(x => x.Total);

                    if (total != 0)
                    {
                        paymentMethodItems.Add(new DailyReceiptDto
                        {
                            Total = total,
                            PaymentMethod = $"Total {paymentMethod}"
                        });
                    }

                    orderedList.AddRange(paymentMethodItems);
                }

                return orderedList;


            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred while getting daily recepit", ex.Message);
            }
        }
    }
}
