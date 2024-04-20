using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using AccountingBlueBook.Authorization.Users;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingBlueBook.GeneralLedgers
{
    public class GeneralLedgerAppService : AccountingBlueBookAppServiceBase, IGeneralLedgerAppService
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

        public GeneralLedgerAppService(IRepository<GeneralLedger, long> generalLedgerRepository,
                                       IRepository<GeneralLedgerDetails, long> generalLedgerDetailRepository,
                                       IRepository<ChartOfAccount> chartOfAccountRepository,
                                       IRepository<ProductService> productServiceRepository,
                                       IRepository<Customer> customerRepository,
                                       IRepository<Invoice, long> invoiceRepository,
                                       IRepository<InvoiceDetail, long> invoiceDetailRepository,
                                       IRepository<LedgerHeaders, long> ledgerHeaderRepository,
                                       IRepository<Company> companyRepository,
                                       IRepository<User, long> userRepository)
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
        }

        public async Task AddLedger(AddGeneralLedgarInputDto Input)
        {
            try
            {
                switch (Input.ProcessType)
                {
                    case "Invoice":
                        Invoice InvoiceObj = await _InvoiceRepository.FirstOrDefaultAsync(Input.InvoiceId);
                        List<InvoiceDetail> InvoiceDetails = await _InvoiceDetailRepository.GetAllListAsync(x => x.InvoiceId == InvoiceObj.Id);
                        List<ProductService> Products = await _ProductServiceRepository.GetAllListAsync(x => InvoiceDetails.Select(y => y.RefProducId).Contains(x.Id));
                        GeneralLedgerDto GeneralLedgerObj = new();
                        GeneralLedgerObj.InvoiceId = Input.InvoiceId;
                        GeneralLedgerObj.InvoiceType = 1;
                        GeneralLedgerObj.Title = "Generation Of Invoice: " + InvoiceObj.InvoiceNo;
                        GeneralLedgerObj.VoucherNo = InvoiceObj.InvoiceNo;
                        GeneralLedgerObj.CreatedBy = InvoiceObj.CreatorUserId;
                        GeneralLedgerObj.CreatorUserName = _UserRepository.FirstOrDefault(x => x.Id == InvoiceObj.CreatorUserId).Name;
                        GeneralLedgerObj.CustomerId = InvoiceObj.RefCustomerId ?? 0;
                        GeneralLedgerObj.Customer = await _CustomerRepository.FirstOrDefaultAsync(Convert.ToInt32(InvoiceObj.RefCustomerId));
                        GeneralLedgerObj.CompanyId = _CompanyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;
                        var LedgerData = await _GeneralLedgerRepository.InsertAsync(ObjectMapper.Map<GeneralLedger>(GeneralLedgerObj));

                        await CurrentUnitOfWork.SaveChangesAsync();

                        var GeneralLedgerDetailList = InvoiceDetails.Select(x => new GeneralLedgerDetails
                        {
                            GeneralLedgerId = LedgerData.Id,
                            DebitAmount = (double)x.Amount,
                            CreditAmount = 0,
                            ChartOfAccountId = (long)Products.FirstOrDefault(y => y.Id == x.RefProducId).IncomeAccountId
                        });

                        await _GeneralLedgerDetailRepository.InsertRangeAsync(GeneralLedgerDetailList);

                        break;

                    case "ReceivedPayment":
                    case "PurchasePayment":
                        Invoice ReceivedPaymentObj = await _InvoiceRepository.FirstOrDefaultAsync(Input.InvoiceId);
                        InvoiceDetail ReceivedPaymentDetails = await _InvoiceDetailRepository.FirstOrDefaultAsync(x => x.InvoiceId == ReceivedPaymentObj.Id);

                        Invoice InvoiceObjRp = await _InvoiceRepository.FirstOrDefaultAsync(x => x.Id == ReceivedPaymentDetails.RefPaidInvoiceId);
                        List<InvoiceDetail> InvoiceDetailsRp = await _InvoiceDetailRepository.GetAllListAsync(x => x.InvoiceId == InvoiceObjRp.Id);



                        foreach (var data in InvoiceDetailsRp)
                        {
                            data.PaidAmount = ReceivedPaymentDetails.PaidAmount;
                        }


                        List<ProductService> ProductsRp = await _ProductServiceRepository.GetAllListAsync(x => InvoiceDetailsRp.Select(y => y.RefProducId).Contains(x.Id));
                        GeneralLedgerDto GeneralLedgerObjRp = new();
                        GeneralLedgerObjRp.InvoiceId = Input.InvoiceId;
                        GeneralLedgerObjRp.InvoiceType = Input.ProcessType == "ReceivedPayment" ? 5 : 11;
                        GeneralLedgerObjRp.Title = Input.ProcessType == "ReceivedPayment" ? "Received Payment: RP-" + InvoiceObjRp.InvoiceNo.Split('-')[1] : "Purchase Payment: " + InvoiceObjRp.InvoiceNo;
                        GeneralLedgerObjRp.VoucherNo = Input.ProcessType == "ReceivedPayment" ? "RP-" + InvoiceObjRp.InvoiceNo.Split('-')[1] : InvoiceObjRp.InvoiceNo;
                        GeneralLedgerObjRp.CreatedBy = InvoiceObjRp.CreatorUserId;
                        GeneralLedgerObjRp.CreatorUserName = _UserRepository.FirstOrDefault(x => x.Id == InvoiceObjRp.CreatorUserId).Name;
                        GeneralLedgerObjRp.CustomerId = Input.ProcessType == "ReceivedPayment" ? InvoiceObjRp.RefCustomerId : null;
                        GeneralLedgerObjRp.Customer = Input.ProcessType == "ReceivedPayment" ? await _CustomerRepository.FirstOrDefaultAsync(Convert.ToInt32(InvoiceObjRp.RefCustomerId)) : null;
                        GeneralLedgerObjRp.CompanyId = _CompanyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;
                        var LedgerDataRp = await _GeneralLedgerRepository.InsertAsync(ObjectMapper.Map<GeneralLedger>(GeneralLedgerObjRp));

                        await CurrentUnitOfWork.SaveChangesAsync();

                        var GeneralLedgerDetailRpList = InvoiceDetailsRp.Select(x => new GeneralLedgerDetails
                        {
                            GeneralLedgerId = LedgerDataRp.Id,
                            DebitAmount = 0,
                            CreditAmount = (double)x.PaidAmount,
                            ChartOfAccountId = (long)ProductsRp.FirstOrDefault(y => y.Id == x.RefProducId).IncomeAccountId
                        });

                        await _GeneralLedgerDetailRepository.InsertRangeAsync(GeneralLedgerDetailRpList);

                        break;



                    case "PurchaseInvoice":
                        Invoice PurchaseInvoiceObj = await _InvoiceRepository.FirstOrDefaultAsync(Input.InvoiceId);
                        List<InvoiceDetail> PurchaseInvoiceDetails = await _InvoiceDetailRepository.GetAllListAsync(x => x.InvoiceId == PurchaseInvoiceObj.Id);
                        List<ProductService> PurchaseProducts = await _ProductServiceRepository.GetAllListAsync(x => PurchaseInvoiceDetails.Select(y => y.RefProducId).Contains(x.Id));
                        GeneralLedgerDto GeneralLedgerObjPI = new();
                        GeneralLedgerObjPI.InvoiceId = Input.InvoiceId;
                        GeneralLedgerObjPI.InvoiceType = 10;
                        GeneralLedgerObjPI.Title = "Generation Of Purchase Invoice: " + PurchaseInvoiceObj.InvoiceNo;
                        GeneralLedgerObjPI.VoucherNo = PurchaseInvoiceObj.InvoiceNo;
                        GeneralLedgerObjPI.CreatedBy = PurchaseInvoiceObj.CreatorUserId;
                        GeneralLedgerObjPI.CreatorUserName = _UserRepository.FirstOrDefault(x => x.Id == PurchaseInvoiceObj.CreatorUserId).Name;
                        GeneralLedgerObjPI.CustomerId = PurchaseInvoiceObj.RefCustomerId;
                        //GeneralLedgerObjPI.Customer = GeneralLedgerObjPI.CustomerId > 0 ? await _CustomerRepository.FirstOrDefaultAsync(Convert.ToInt32(PurchaseInvoiceObj.RefCustomerId)) : new Customer();
                        GeneralLedgerObjPI.CompanyId = _CompanyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;
                        var LedgerDataPI = await _GeneralLedgerRepository.InsertAsync(ObjectMapper.Map<GeneralLedger>(GeneralLedgerObjPI));

                        await CurrentUnitOfWork.SaveChangesAsync();

                        var GeneralLedgerDetailListPI = PurchaseInvoiceDetails.Select(x => new GeneralLedgerDetails
                        {
                            GeneralLedgerId = LedgerDataPI.Id,
                            DebitAmount = (double)x.Amount,
                            CreditAmount = 0,
                            ChartOfAccountId = (long)PurchaseProducts.FirstOrDefault(y => y.Id == x.RefProducId).IncomeAccountId
                        });

                        await _GeneralLedgerDetailRepository.InsertRangeAsync(GeneralLedgerDetailListPI);

                        break;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<GeneralLedgerChartOfAccountData>> CoaData(List<ChartOfAccount> CoaList, List<InvoiceDetail> InvoiceDetailList, List<ProductService> ProductList)
        {

            return InvoiceDetailList.Select(x =>
            {
                var ProductObj = ProductList.FirstOrDefault(y => y.Id == x.RefProducId);

                if (ProductObj != null)
                {
                    var IncomeAccountId = (long)ProductObj.IncomeAccountId;

                    return new GeneralLedgerChartOfAccountData
                    {
                        MainHeadId = (long)CoaList.FirstOrDefault(y => y.Id == IncomeAccountId).MainHead.Id,
                        MainHeadName = CoaList.FirstOrDefault(y => y.Id == IncomeAccountId).MainHead.Name,
                        SubHeadId = IncomeAccountId,
                        SubHeadName = CoaList.FirstOrDefault(y => y.Id == IncomeAccountId).AccountDescription,
                        Amount = (double)x.Amount
                    };
                }
                else
                {
                    return null; // Handle the case where the product is not found
                }
            }).Where(x => x != null).ToList();


        }

        [HttpPost]
        public async Task<List<GeneralLedgerOutputDto>> GetLedgerForTable(GetGeneralLedgerInputDto input)
        {
            var CompanyId = _CompanyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;

            var FilteredLedger = await _GeneralLedgerRepository.GetAll().Where(x => x.CompanyId == CompanyId).Include(x => x.Customer).Include(x => x.GeneralLedgerDetails).ToListAsync();

            var GeneralLedgerDetails = FilteredLedger.SelectMany(x => x.GeneralLedgerDetails).Distinct(); //await _GeneralLedgerDetailRepository.GetAllListAsync(x => FilteredLedger.Select(y => y.Id).Contains(x.GeneralLedgerId));

            var CoaList = await _ChartOfAccountRepository.GetAll().Where(x => GeneralLedgerDetails.Select(y => y.ChartOfAccountId).Contains(x.Id)).Include(x => x.MainHead).ToListAsync();


            var InvoiceDetailListMaster = await _InvoiceDetailRepository.GetAllListAsync(x => FilteredLedger.Select(y => y.InvoiceId).Contains(x.InvoiceId));

            InvoiceDetailListMaster.AddRange(await _InvoiceDetailRepository.GetAllListAsync(x => InvoiceDetailListMaster.Select(y => y.RefPaidInvoiceId).Contains(x.InvoiceId)));



            var ProductListMaster = await _ProductServiceRepository.GetAllListAsync(x => InvoiceDetailListMaster.Select(y => y.RefProducId).Contains(x.Id));

            double previousBalance = 0;

            List<GeneralLedgerOutputDto> LedgerList = new();

            foreach (var Data in CoaList) {

                var CoaDataObj = new GeneralLedgerChartOfAccountData
                {
                    SubHeadId = Data.Id,
                    SubHeadName = Data.AccountDescription,
                    MainHeadId = (int)Data.MainHeadId,
                };
                var CoaOutputObj = new GeneralLedgerOutputDto
                {
                    Id = 0,
                    ChartOfAccountData = new List<GeneralLedgerChartOfAccountData> { CoaDataObj },
                    LinkedSubHeadId = Data.Id,
                    Type = "Header",
                };
                LedgerList.Add(CoaOutputObj);

                var QueryData = await Task.WhenAll(FilteredLedger.Where(x => x.GeneralLedgerDetails.Any(y => y.ChartOfAccountId == Data.Id)).Select(async (x, index) =>
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
                    var CoaRes = await CoaData(CoaList, InvoiceDetailList, ProductList);

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
                        LinkedSubHeadId = Data.Id,
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
                    ChartOfAccountData = CoaOutputObj.ChartOfAccountData,
                    LinkedSubHeadId = Data.Id
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

        public async Task SaveHeaders(string Headers, int CustomerId, string Type)
        {
            var LedgerHeadersObj = await _LedgerHeaderRepository.FirstOrDefaultAsync(x => x.CustomerId == CustomerId && x.LedgerType == Type);
            var CompanyId = _CompanyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;
            if (LedgerHeadersObj == null)
            {
                LedgerHeaders LedgerHeader = new LedgerHeaders();
                LedgerHeader.CustomerId = CustomerId;
                LedgerHeader.Headers = Headers;
                LedgerHeader.CompanyId = CompanyId;
                LedgerHeader.LedgerType = Type;
                await _LedgerHeaderRepository.InsertAsync(LedgerHeader);
            }
            else
            {
                LedgerHeadersObj.CustomerId = CustomerId;
                LedgerHeadersObj.CompanyId = CompanyId;
                LedgerHeadersObj.Headers = Headers;
                await _LedgerHeaderRepository.UpdateAsync(LedgerHeadersObj);
            }
        }

        public async Task<LedgerHeaders> GetHeaders(int CustomerId, string Type)
        {
            return await _LedgerHeaderRepository.FirstOrDefaultAsync(x => x.CustomerId == CustomerId && x.LedgerType == Type);

        }
    }
}
