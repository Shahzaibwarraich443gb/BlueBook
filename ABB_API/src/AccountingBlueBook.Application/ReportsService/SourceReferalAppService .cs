using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AccountingBlueBook.Entities.MainEntities.Reports;
using AccountingBlueBook.Entities.MainEntities.Reports.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.ReportsService
{
    [AbpAuthorize]
    public class SourceReferalAppService : AccountingBlueBookAppServiceBase
    {
        private readonly IReportRepository _reportRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<SourceReferralType> _sourceReferaltypes;

        public SourceReferalAppService(IReportRepository reportRepository, IRepository<Customer> customerRepository, IRepository<Transaction> transactionRepository,
            IRepository<Company> companyRepository, IRepository<SourceReferralType> sourceReferaltypes)
        {
            _reportRepository = reportRepository;
            _companyRepository = companyRepository;
            _customerRepository = customerRepository;
            _transactionRepository = transactionRepository;
            _sourceReferaltypes = sourceReferaltypes;
        }

        public async Task<List<SourceReferalDto>> GetList(DateTime _StartDate, DateTime _EndDate, long SoureceReferalId )
        {
            try
            {

                //var query =await  _customerRepository.GetAll().Where(x => x.SourceReferralTypeId == SoureceReferalId).ToDynamicListAsync();

                //var customerIds = query.Select(customer => customer.Id).ToList();

                //var transactionsForMatchingCustomers = await _transactionRepository.GetAll()
                //    .Where(transaction => customerIds.Contains(transaction.RefCustomerID)).ToDynamicListAsync();
               // var tenatid = AbpSession.TenantId == null ? 1 : (int)AbpSession.TenantId;
                var res = await _reportRepository.GetAllSourceReferal(_StartDate, _EndDate,  SoureceReferalId);
                return res;



            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred while getting EmployeeActivities", ex.Message);
            }

        }
        public async Task<List<SourceReferalDto>> GetSourceReferralNamesAsync()
        {
            var returnResult = await _sourceReferaltypes.GetAll().Select(x => new SourceReferalDto { Id = x.Id, CompanyName = x.Name }).ToListAsync();
            return returnResult;
              
                
        }

    }
}
