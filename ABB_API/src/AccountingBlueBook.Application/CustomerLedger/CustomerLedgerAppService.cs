using Abp.Domain.Repositories;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AccountingBlueBook.Authorization.Users;
using Abp.Runtime.Session;
using AccountingBlueBook.GeneralLedgers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace AccountingBlueBook.CustomerLedger
{
    public class CustomerLedgerAppService : AccountingBlueBookAppServiceBase, ICustomerLedgerAppService
    {
        private readonly IRepository<GeneralLedger, long> _GeneralLedgerRepository;
        private readonly IRepository<GeneralLedgerDetails, long> _GeneralLedgerDetailRepository;
        private readonly IRepository<ChartOfAccount> _ChartOfAccountRepository;
        private readonly IRepository<ProductService> _ProductServiceRepository;
        private readonly IRepository<Customer> _CustomerRepository;
        private readonly IRepository<Invoice, long> _InvoiceRepository;
        private readonly IRepository<InvoiceDetail, long> _InvoiceDetailRepository;
        private readonly IRepository<LedgerHeaders, long> _LedgerHeaderRepository;
        private readonly IRepository<Company> _CompanyRepository;
        private readonly IRepository<User, long> _UserRepository;
        private readonly IGeneralLedgerAppService _GeneralLedgerAppService;

        public CustomerLedgerAppService(IRepository<GeneralLedger, long> generalLedgerRepository,
                                       IRepository<GeneralLedgerDetails, long> generalLedgerDetailRepository,
                                       IRepository<ChartOfAccount> chartOfAccountRepository,
                                       IRepository<ProductService> productServiceRepository,
                                       IRepository<Customer> customerRepository,
                                       IRepository<Invoice, long> invoiceRepository,
                                       IRepository<InvoiceDetail, long> invoiceDetailRepository,
                                       IRepository<LedgerHeaders, long> ledgerHeaderRepository,
                                       IRepository<Company> companyRepository,
                                       IRepository<User, long> userRepository,
                                       IGeneralLedgerAppService generalLedgerAppService)
        {
            _GeneralLedgerRepository = generalLedgerRepository;
            _GeneralLedgerDetailRepository = generalLedgerDetailRepository;
            _ChartOfAccountRepository = chartOfAccountRepository;
            _ProductServiceRepository = productServiceRepository;
            _CustomerRepository = customerRepository;
            _InvoiceRepository = invoiceRepository;
            _InvoiceDetailRepository = invoiceDetailRepository;
            _LedgerHeaderRepository = ledgerHeaderRepository;
            _CompanyRepository = companyRepository;
            _UserRepository = userRepository;
            _GeneralLedgerAppService = generalLedgerAppService;
        }


        [HttpPost]
        public async Task<List<GeneralLedgerOutputDto>> GetLedgerForTable(GetGeneralLedgerInputDto input)
        {
            var CompanyId = _CompanyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;

            var FilteredLedger = await _GeneralLedgerRepository.GetAll().Where(x => x.CompanyId == CompanyId).Include(x => x.Customer).Include(x => x.GeneralLedgerDetails).ToListAsync();

            var GeneralLedgerDetails = FilteredLedger.SelectMany(x => x.GeneralLedgerDetails).ToList(); //await _GeneralLedgerDetailRepository.GetAllListAsync(x => FilteredLedger.Select(y => y.Id).Contains(x.GeneralLedgerId));

            var CustomerList = FilteredLedger.Select(x => x.Customer).Distinct();

            var CoaList = await _ChartOfAccountRepository.GetAll().Where(x => GeneralLedgerDetails.Select(y => y.ChartOfAccountId).Contains(x.Id)).Include(x => x.MainHead).ToListAsync();


            var InvoiceDetailListMaster = await _InvoiceDetailRepository.GetAllListAsync(x => FilteredLedger.Select(y => y.InvoiceId).Contains(x.InvoiceId));

            InvoiceDetailListMaster.AddRange(await _InvoiceDetailRepository.GetAllListAsync(x => InvoiceDetailListMaster.Select(y => y.RefPaidInvoiceId).Contains(x.InvoiceId)));



            var ProductListMaster = await _ProductServiceRepository.GetAllListAsync(x => InvoiceDetailListMaster.Select(y => y.RefProducId).Contains(x.Id));

            double previousBalance = 0;

            List<GeneralLedgerOutputDto> LedgerList = new();

            foreach (var Data in CustomerList)
            {

                var OutputObj = new GeneralLedgerOutputDto
                {
                    Id = 0,
                    CustomerId = Data.Id,
                    CustomerName = Data.Name,
                    CompanyName = Data.BussinessName,
                    Type = "Header",
                };
                LedgerList.Add(OutputObj);

                var QueryData = await Task.WhenAll(FilteredLedger.Where(x => x.CustomerId == Data.Id).Select(async (x, index) =>
                {
                    List<InvoiceDetail> InvoiceDetailList = new();

                    int[] UnpaidInvoiceTypes = { 1, 2, 3, 8, 10 };

                    if (UnpaidInvoiceTypes.Any(y => y == x.InvoiceType))
                    {
                        InvoiceDetailList = InvoiceDetailListMaster.Where(y => y.InvoiceId == x.InvoiceId).ToList();
                    }
                    else
                    {
                        InvoiceDetailList = InvoiceDetailListMaster.Where(y => y.InvoiceId == InvoiceDetailListMaster.FirstOrDefault(z => z.InvoiceId == x.InvoiceId).RefPaidInvoiceId).ToList();
                    }
                    var ProductList = ProductListMaster.Where(x => InvoiceDetailList.Select(y => y.RefProducId).Contains(x.Id)).ToList();
                    var CoaRes = await _GeneralLedgerAppService.CoaData(CoaList, InvoiceDetailList, ProductList);

                    var PrevCreditAmount = index == 0 ? 0 : GeneralLedgerDetails.FirstOrDefault(y => y.GeneralLedgerId == FilteredLedger.ElementAt(index - 1).Id).CreditAmount;  //getting from ledger details for previous 

                    var PrevDebitAmount = index == 0 ? 0 : GeneralLedgerDetails.FirstOrDefault(y => y.GeneralLedgerId == FilteredLedger.ElementAt(index - 1).Id).DebitAmount;

                    var CurrentDebitAmount = GeneralLedgerDetails.FirstOrDefault(y => y.GeneralLedgerId == x.Id).DebitAmount;

                    var CurrentCreditAmount = GeneralLedgerDetails.FirstOrDefault(y => y.GeneralLedgerId == x.Id).CreditAmount;

                    var CurrentBalance = Math.Round(CurrentDebitAmount - CurrentCreditAmount, 2);

                    //open balance is previous balance
                    var OpenBalance = index == 0 ? x.Balance : Math.Round(previousBalance + (PrevDebitAmount - PrevCreditAmount), 2); //Math.Round(FilteredLedger.ElementAt(index - 1).Balance)+(PrevDebitAmount - PrevCreditAmount), 2);

                    previousBalance = OpenBalance;

                    return new GeneralLedgerOutputDto
                    {
                        Id = x.Id,
                        DateAlt = x.CreationTime.ToString("MM/dd/yyyy"),
                        Description = x.Title,
                        CustomerName = x.Customer != null ? x.Customer.Name : "",
                        CompanyName = x.Customer != null ? x.Customer.BussinessName : "",
                        VoucherId = x.VoucherNo,
                        OpeningBalance = OpenBalance,
                        DebitAmount = CurrentDebitAmount,
                        CreditAmount = CurrentCreditAmount,
                        Balance = index == 0 ? CurrentBalance : Math.Round(OpenBalance + CurrentBalance, 2),
                        CustomerId = x.CustomerId == null ? 0 : (long)x.CustomerId,
                        CreationTime = x.CreationTime,
                        ChartOfAccountData = CoaRes,
                        CSR = x.CreatorUserName,
                        Type = "Data"
                    };
                }));

                LedgerList.AddRange(QueryData.Where(x => x.CreationTime.Date >= input.StartDate.Date && x.CreationTime.Date <= input.EndDate.Date).OrderByDescending(x => x.CreationTime).ToList());


                var SumObj = new GeneralLedgerOutputDto
                {
                    Type = "Sum",
                    CreditAmount = Math.Round(QueryData.Sum(x => x.CreditAmount), 2),
                    DebitAmount = Math.Round(QueryData.Sum(x => x.DebitAmount), 2),
                    CustomerId = OutputObj.CustomerId,
                    CompanyName = OutputObj.CompanyName
                };

                LedgerList.Add(SumObj);
            }

            //foreach (var Data in CoaList)
            //{
            //    var CoaData = new GeneralLedgerChartOfAccountData
            //    {
            //        SubHeadId = Data.Id,
            //        SubHeadName = Data.AccountDescription,
            //        MainHeadId = (int)Data.MainHeadId,
            //    };
            //    var Obj = new GeneralLedgerOutputDto
            //    {
            //        Id = 0,
            //        ChartOfAccountData = new List<GeneralLedgerChartOfAccountData> { CoaData },
            //        Type = "Header",
            //    };
            //    LedgerList.Add(Obj);
            //    LedgerList.AddRange(QueryData.Where(x => x.ChartOfAccountData.Any(y => y.SubHeadId == CoaData.SubHeadId) &&
            //                                             x.CreationTime.Date >= input.StartDate.Date &&
            //                                             x.CreationTime.Date <= input.EndDate.Date)
            //                                  .OrderByDescending(x => x.CreationTime));
            //}


            //LedgerList = QueryData.Where(x => x.CreationTime.Date >= input.StartDate.Date && x.CreationTime.Date <= input.EndDate.Date).OrderByDescending(x => x.CreationTime).ToList();

            return LedgerList;

        }

    }
}
