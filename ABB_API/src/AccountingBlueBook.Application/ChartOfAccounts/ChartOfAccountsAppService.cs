using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using AccountingBlueBook.AccountTypes.Dto;
using AccountingBlueBook.ChartOfAccounts.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Enums;
using AccountingBlueBook.MainHeading.Dto;
using AccountingBlueBook.Users.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System;
using Abp.EntityFrameworkCore.Repositories;
using AccountingBlueBook.Common.CommonLookupDto;
using Castle.Core.Resource;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using Microsoft.AspNetCore.Http.HttpResults;
using Org.BouncyCastle.Math.EC.Rfc7748;
using AccountingBlueBook.AppServices.ProductServices.Dto;
using AccountingBlueBook.AppServices.Invoices.dto;

namespace AccountingBlueBook.ChartOfAccounts
{
    public class ChartOfAccountsAppService : AccountingBlueBookAppServiceBase, IContactPersonTypeAppService
    {
        private readonly IRepository<ChartOfAccount> _chartOfAccountRepository;
        private readonly IRepository<AccountType> _accountTypesRepository;
        private readonly IRepository<MainHead> _mainHeadRepository;
        private readonly IRepository<InvoiceDetail, long> _invoiceDetailRepository;
        private readonly IRepository<Invoice, long> _invoiceRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<ProductService> _productServiceRepository;
        private IObjectMapper ObjectMapper;

        public ChartOfAccountsAppService(
            IRepository<ChartOfAccount> chartOfAccountRepository,
            IObjectMapper objectMapper,
            IRepository<AccountType> accountTypesRepository,
            IRepository<MainHead> mainHeadRepository,
            IRepository<InvoiceDetail, long> invoiceDetailRepository,
            IRepository<Invoice, long> invoiceRepository,
            IRepository<Company> companyRepository,
            IRepository<ProductService> productServiceRepository
            )
        {
            _chartOfAccountRepository = chartOfAccountRepository;
            ObjectMapper = objectMapper;
            _accountTypesRepository = accountTypesRepository;
            _mainHeadRepository = mainHeadRepository;
            _invoiceDetailRepository = invoiceDetailRepository;
            _invoiceRepository = invoiceRepository;
            _companyRepository = companyRepository;
            _productServiceRepository = productServiceRepository;
        }

        public async Task CreateOrEditChartOfAccount(CreateOrEditChartOfAccountInputDto input)
        {
            if (input.Id > 0)
                await EditChartOfAccount(input);
            else
                await CreateChartOfAccount(input);
        }

        private async Task EditChartOfAccount(CreateOrEditChartOfAccountInputDto input)
        {
            var chartOfAccount = await _chartOfAccountRepository.FirstOrDefaultAsync((int)input.Id);
            chartOfAccount.AccountNature = input.AccountNature;
            chartOfAccount.AccountTypeId = input.AccountTypeId;
            chartOfAccount.AccountCode = input.AccountCode;
            chartOfAccount.MainHeadId = input.MainHeadId;
            chartOfAccount.AccountDescription = input.AccountDescription;
            chartOfAccount.IsActive = true;

            await _chartOfAccountRepository.UpdateAsync(chartOfAccount);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private async Task CreateChartOfAccount(CreateOrEditChartOfAccountInputDto input)
        {
            try
            {
                var chartOfAccount = ObjectMapper.Map<ChartOfAccount>(input);
                chartOfAccount.AccountNature = input.AccountNature;
                chartOfAccount.CompanyId = _companyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;
                chartOfAccount.IsActive = true;
                await _chartOfAccountRepository.InsertAsync(chartOfAccount);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> DeletChartOfAccounts(EntityDto input)
        {
            var chartOfAccount = await _chartOfAccountRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            if (chartOfAccount.MainHeadId == 1 || chartOfAccount.MainHeadId == 4)
            {
                return "Cannot Delete As It Is A Master Chart Of Account";
            }

            if (_productServiceRepository.GetAll().Any(x => x.IncomeAccountId == chartOfAccount.Id || x.ExpenseAccountId == chartOfAccount.Id))
            {
                return "Cannot Delete As It Is Being Used By A Product";
            }
            await _chartOfAccountRepository.DeleteAsync(chartOfAccount);
            await CurrentUnitOfWork.SaveChangesAsync();
            return "Deleted Successfully";
        }

        public async Task<ChartOfAccountDto> GetChartOfAccounts(EntityDto input)
        {
            var chartOfAccount = await _chartOfAccountRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            var data = ObjectMapper.Map<ChartOfAccountDto>(chartOfAccount);
            data.AccountNatureId = (int)chartOfAccount.AccountNature;
            return data;
        }

        public async Task ChangeCoaBalance(List<InvoiceDetailDto>? detailsInput, string type, int bankCoaId = 0, decimal previousAmount = 0)
        {
            //getting products of detailsInput from db
            var prodList = await _productServiceRepository.GetAll().Where(x => detailsInput.Select(y => y.RefProducID).ToList().Contains(x.Id)).ToListAsync();

            bool isReceivedPaymentEdit = false;

            if (prodList.Count() == 0 && type == "ReceivedPayment")
            {
                isReceivedPaymentEdit = true;
                var invoiceObj = await _invoiceRepository.FirstOrDefaultAsync(x => x.InvoiceNo == detailsInput[0].InvoiceNo);
                var invoiceDetailsList = await _invoiceDetailRepository.GetAllListAsync(x => x.InvoiceId == invoiceObj.Id);
                var refInvoiceDetailList = await _invoiceDetailRepository.GetAll().Where(x => invoiceDetailsList.Select(y => y.RefPaidInvoiceId).ToList().Contains(x.InvoiceId)).ToListAsync();
                prodList = await _productServiceRepository.GetAll().Where(x => refInvoiceDetailList.Select(y => y.RefProducId).ToList().Contains(x.Id)).ToListAsync();
            }

            //making a custom list for coa from prodList
            var dataList = new List<ChartOfAccountInputDto>();

            prodList.ForEach(data =>
            {
                var obj = new ChartOfAccountInputDto
                {
                    IncomeAccountId = data.IncomeAccountId != null ? (long)data.IncomeAccountId : 0,
                    ExpenseAccountId = data.ExpenseAccountId != null ? (long)data.ExpenseAccountId : 0,
                    LiabilityAccountId = data.LiabilityAccountId ?? 0,
                    AdvSaleTaxAccountId = data.AdvanceSaleTaxAccountId ?? 0,
                    Amount = (decimal)detailsInput.Where(x => x.RefProducID == data.Id).Sum(x => x.Amount),
                    PaidAmount = isReceivedPaymentEdit == false ? (decimal)detailsInput.Where(x => x.RefProducID == data.Id).Sum(x => x.PaidAmount) : (decimal)detailsInput[0].PaidAmount / prodList.Count(),
                    AdvancedSaleTax = Convert.ToDecimal(data.SaleTax != null ? data.SaleTax : 0),
                    LiabilitySaleTax = Convert.ToDecimal(data.ExpenseSaleTax != null ? data.ExpenseSaleTax : 0),
                    Rate = (decimal)detailsInput.Where(x => x.RefProducID == data.Id).Sum(x => x.Rate)
                };
                dataList.Add(obj);
            });



            switch (type)
            {


                case "Invoice":
                    ChartOfAccount AccountReceivableInvoice = await _chartOfAccountRepository.FirstOrDefaultAsync(x => x.AccountDescription == "Customer Receivable" && (x.Id == 82 || x.Id == 70));
                    AccountReceivableInvoice.Balance += dataList.Sum(x => x.Amount);


                    AccountReceivableInvoice.Balance -= previousAmount; //in case of update remove previous values


                    await _chartOfAccountRepository.UpdateAsync(AccountReceivableInvoice);

                    var incomeAccountsInvoice = await _chartOfAccountRepository.GetAllListAsync(x => dataList.Select(y => y.IncomeAccountId).ToList().Contains(x.Id));


                    var liabilityAccountsInvoice = await _chartOfAccountRepository.GetAllListAsync(x => dataList.Select(y => y.LiabilityAccountId).ToList().Contains(x.Id));

                    var AdvSaleTaxAccountsInvoice = await _chartOfAccountRepository.GetAllListAsync(x => dataList.Select(y => y.AdvSaleTaxAccountId).ToList().Contains(x.Id));



                    dataList.ForEach(data =>
                    {
                        if (data.IncomeAccountId > 0)
                        {
                            incomeAccountsInvoice.FirstOrDefault(x => x.Id == data.IncomeAccountId).Balance -= previousAmount; //in case of update remove previous values
                            incomeAccountsInvoice.FirstOrDefault(x => x.Id == data.IncomeAccountId).Balance += data.Amount;



                            //for sale tax accounts

                            if (previousAmount == 0)
                            {
                                if (data.LiabilityAccountId != null && data.LiabilityAccountId > 0)
                                {
                                    liabilityAccountsInvoice.FirstOrDefault(x => x.Id == data.LiabilityAccountId).Balance += data.Rate * (data.LiabilitySaleTax/100) ?? 0;
                                }

                                if (data.AdvSaleTaxAccountId != null && data.AdvSaleTaxAccountId > 0)
                                {
                                    AdvSaleTaxAccountsInvoice.FirstOrDefault(x => x.Id == data.AdvSaleTaxAccountId).Balance -= data.Rate * (data.AdvancedSaleTax/100) ?? 0;
                                }
                            }

                        }
                    });
                    break;



                case "ReceivedPayment":
                    ChartOfAccount AccountReceivablePayment = await _chartOfAccountRepository.FirstOrDefaultAsync(x => x.AccountDescription == "Customer Receivable" && (x.Id == 82 || x.Id == 70));
                    AccountReceivablePayment.Balance -= dataList.Sum(x => x.PaidAmount);

                    AccountReceivablePayment.Balance += previousAmount; //in case of update add previous amount in invoice and remove from the new recieved payment


                    ChartOfAccount BankCashReceivedPayment = await _chartOfAccountRepository.FirstOrDefaultAsync(x => x.Id == bankCoaId);
                    BankCashReceivedPayment.Balance += dataList.Sum(x => x.PaidAmount);

                    await _chartOfAccountRepository.UpdateAsync(AccountReceivablePayment);

                    var incomeAccountsReceivedPayment = await _chartOfAccountRepository.GetAllListAsync(x => dataList.Select(y => y.IncomeAccountId).ToList().Contains(x.Id));


                    var liabilityAccountsReceivedPayment = await _chartOfAccountRepository.GetAllListAsync(x => dataList.Select(y => y.LiabilityAccountId).ToList().Contains(x.Id));

                    var AdvSaleTaxAccountsReceivedPayment = await _chartOfAccountRepository.GetAllListAsync(x => dataList.Select(y => y.AdvSaleTaxAccountId).ToList().Contains(x.Id));




                    dataList.ForEach(data =>
                    {
                        if (data.IncomeAccountId > 0)
                        {
                            incomeAccountsReceivedPayment.FirstOrDefault(x => x.Id == data.IncomeAccountId).Balance -= data.PaidAmount;
                            incomeAccountsReceivedPayment.FirstOrDefault(x => x.Id == data.IncomeAccountId).Balance += previousAmount;  //in case of update remove previous amount


                            //for sale tax accounts

                            if (previousAmount == 0)
                            {
                                if (data.LiabilityAccountId != null && data.LiabilityAccountId > 0)
                                {
                                    liabilityAccountsReceivedPayment.FirstOrDefault(x => x.Id == data.LiabilityAccountId).Balance -= data.Rate * (data.LiabilitySaleTax / 100) ?? 0;
                                }

                                if (data.AdvSaleTaxAccountId != null && data.AdvSaleTaxAccountId > 0)
                                {

                                    AdvSaleTaxAccountsReceivedPayment.FirstOrDefault(x => x.Id == data.AdvSaleTaxAccountId).Balance += data.Rate * (data.AdvancedSaleTax / 100) ?? 0;
                                }
                            }
                        }
                    });
                    break;




                case "PurchaseInvoice":
                    ChartOfAccount AccountPayableInvoice = await _chartOfAccountRepository.FirstOrDefaultAsync(x => x.AccountDescription == "Vendor Payable" && (x.Id == 69 || x.Id == 84));

                    AccountPayableInvoice.Balance -= dataList.Sum(x => x.Amount);


                    await _chartOfAccountRepository.UpdateAsync(AccountPayableInvoice);

                    var expenseAccountsInvoice = await _chartOfAccountRepository.GetAllListAsync(x => dataList.Select(y => y.ExpenseAccountId).ToList().Contains(x.Id));

                    dataList.ForEach(data =>
                    {
                        if (data.ExpenseAccountId > 0)
                        {

                            expenseAccountsInvoice.FirstOrDefault(x => x.Id == data.ExpenseAccountId).Balance += data.Amount;
                        }
                    });
                    break;




                case "PurchasePayment":
                    ChartOfAccount AccountPayablePurchasePayment = await _chartOfAccountRepository.FirstOrDefaultAsync(x => x.AccountDescription == "Vendor Payable" && (x.Id == 69 || x.Id == 84));
                    AccountPayablePurchasePayment.Balance += dataList.Sum(x => x.PaidAmount);

                    ChartOfAccount BankCashPurchasePayment = await _chartOfAccountRepository.FirstOrDefaultAsync(x => x.Id == bankCoaId);
                    BankCashPurchasePayment.Balance -= dataList.Sum(x => x.PaidAmount);

                    await _chartOfAccountRepository.UpdateAsync(AccountPayablePurchasePayment);

                    var expenseAccountsPurchasePayment = await _chartOfAccountRepository.GetAllListAsync(x => dataList.Select(y => y.ExpenseAccountId).ToList().Contains(x.Id));

                    dataList.ForEach(data =>
                    {
                        if (data.ExpenseAccountId > 0)
                        {
                            expenseAccountsPurchasePayment.FirstOrDefault(x => x.Id == data.ExpenseAccountId).Balance -= data.PaidAmount;
                        }
                    });
                    break;



                case "JournalVoucher":
                    List<ChartOfAccount> chartOfAccountList = await _chartOfAccountRepository.GetAllListAsync(x => detailsInput.Select(y => y.RefChartOfAccountID).ToList().Contains(x.Id));


                    detailsInput.ForEach(data =>
                    {
                        var coaObj = chartOfAccountList.FirstOrDefault(x => x.Id == data.RefChartOfAccountID);
                        coaObj.Balance += (long)data.Amount; //credit amount
                        coaObj.Balance -= (long)data.PaidAmount; //debit amount

                    });

                    break;


            }

        }


        public async Task<List<ChartOfAccountDto>> GetChartOfAccountList()
        {
            int companyId = _companyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;
            var data = await _chartOfAccountRepository.GetAllListAsync(x => x.CompanyId == companyId);
            var chartOfAccountList = ObjectMapper.Map<List<ChartOfAccountDto>>(data);
            var mainHead = await _mainHeadRepository.GetAllListAsync();
            var accountType = await _accountTypesRepository.GetAllListAsync();

            chartOfAccountList.ForEach(c =>
            {


                c.MainHeadCode = mainHead.FirstOrDefault(x => x.Id == c.MainHeadId).Code;
                c.MainHead = mainHead.FirstOrDefault(x => x.Id == c.MainHeadId).Name;
                c.AccountTypeCode = accountType.FirstOrDefault(x => x.Id == c.AccountTypeId).Code;
                c.AccountType = accountType.FirstOrDefault(x => x.Id == c.AccountTypeId).Name;
                c.AccountNatureId = (int)(AccountNature)Enum.Parse(typeof(AccountNature), c.AccountNature);
            });

            return chartOfAccountList;
        }



        //   public async Task<List<ChartOfAccountDto>> GetChartOfAccountList()
        //   {
        //       int companyId = _companyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;
        //       var data = await _chartOfAccountRepository.GetAllListAsync(x => x.CompanyId == companyId);
        //       var chartOfAccountList = ObjectMapper.Map<List<ChartOfAccountDto>>(data);


        //       var invoiceIdListUnpaid = await _invoiceDetailRepository.GetAll().Where(x => x.RefPaidInvoiceId == null).ToListAsync();

        //       var recievedPaymentList = await _invoiceDetailRepository.GetAll().Where(x => x.RefPaidInvoiceId != null).ToListAsync(); //paid invoices are those which are in refpaidinvoiceid

        //       var receivedPaymentIds = recievedPaymentList.Select(x => x.Id).ToList();

        //       // Remove the IDs that are present in the received payment list from the unpaid invoice list
        //       invoiceIdListUnpaid = invoiceIdListUnpaid.Where(x => !receivedPaymentIds.Contains(x.Id)).ToList();


        //       List<dynamic> coaUnPaidProductList = new();
        //       List<dynamic> coaPaidProductList = new();

        //       var productServices = await _productServiceRepository.GetAll()
        //.Where(x => invoiceIdListUnpaid.Select(y => y.RefProducId).Contains(x.Id))
        //.ToListAsync();



        //       invoiceIdListUnpaid.ForEach(data =>
        //       {
        //           var p = productServices.FirstOrDefault(x => x.Id == data.RefProducId);

        //           if (p != null)
        //           {
        //               var obj = new
        //               {
        //                   //coaId = p.IncomeAccountId != null ? p.IncomeAccountId : p.ExpenseAccountId,
        //                   coaIdIncome = p.IncomeAccountId,
        //                   coaIdExpense = p.ExpenseAccountId,
        //                   amount = data.Amount
        //               };

        //               coaUnPaidProductList.Add(obj);
        //           }
        //       });


        //       List<dynamic> coaDepositIds = new List<dynamic>();

        //       recievedPaymentList.ForEach(data =>
        //       {
        //           var invoiceObj = invoiceIdListUnpaid.FirstOrDefault(x => x.InvoiceId == data.RefPaidInvoiceId);
        //           if (invoiceObj != null)
        //           {
        //               var p = productServices.FirstOrDefault(x => x.Id == invoiceObj.RefProducId);
        //               var obj = new
        //               {
        //                   //coaId = p.IncomeAccountId != null ? p.IncomeAccountId : p.ExpenseAccountId,
        //                   coaIdIncome = p.IncomeAccountId,
        //                   coaIdExpense = p.ExpenseAccountId,
        //                   amount = data.PaidAmount
        //               };

        //               if (data.RefChartOfAccountId != null)
        //               {
        //                   var CoaObj = new
        //                   {
        //                       cId = data.RefChartOfAccountId,
        //                       amount = data.PaidAmount
        //                   };
        //                   coaDepositIds.Add(CoaObj);
        //               }

        //               coaPaidProductList.Add(obj);
        //           }
        //       });



        //       var mainHead = await _mainHeadRepository.GetAllListAsync();
        //       var accountType = await _accountTypesRepository.GetAllListAsync();

        //       chartOfAccountList.ForEach(c =>
        //       {
        //           c.Balance = 0;

        //           //todo : implement for multiple companies (create coa)
        //           if (c.Id == 70 && c.AccountDescription == "Customer Receivable") //82 for dev and 70 for prod
        //           {
        //               c.Balance += invoiceIdListUnpaid.Sum(invoiceData => invoiceData.Amount ?? 0);
        //               c.Balance -= recievedPaymentList.Sum(recievedData => recievedData.PaidAmount ?? 0);
        //           }
        //           else if(c.Id == 82 && c.AccountDescription == "Customer Receivable")
        //           {
        //               c.Balance += invoiceIdListUnpaid.Sum(invoiceData => invoiceData.Amount ?? 0);
        //               c.Balance -= recievedPaymentList.Sum(recievedData => recievedData.PaidAmount ?? 0);
        //           }
        //           else
        //           {
        //               var paid = coaUnPaidProductList.Where(x => x.coaIdIncome == c.Id).Sum(x => (decimal)x.amount);
        //               var unpaid = coaPaidProductList.Where(x => x.coaIdIncome == c.Id).Sum(x => (decimal)x.amount);
        //               c.Balance = Math.Abs(coaUnPaidProductList.Where(x => x.coaIdIncome == c.Id).Sum(x => (decimal)x.amount) - coaPaidProductList.Where(x => x.coaIdIncome == c.Id).Sum(x => (decimal)x.amount));
        //           }

        //           if (coaDepositIds.Any(x => x.cId == c.Id))
        //           {
        //               c.Balance += coaDepositIds.Where(x => x.cId == c.Id).Sum(x => x.amount != null ? (decimal)x.amount : 0);
        //           }


        //           c.MainHeadCode = mainHead.FirstOrDefault(x => x.Id == c.MainHeadId).Code;
        //           c.MainHead = mainHead.FirstOrDefault(x => x.Id == c.MainHeadId).Name;
        //           c.AccountTypeCode = accountType.FirstOrDefault(x => x.Id == c.AccountTypeId).Code;
        //           c.AccountType = accountType.FirstOrDefault(x => x.Id == c.AccountTypeId).Name;
        //           c.AccountNatureId = (int)(AccountNature)Enum.Parse(typeof(AccountNature), c.AccountNature);
        //       });

        //       return chartOfAccountList;
        //   }




        public async Task<List<ChartOfAccountDto>> GetChartOfAccountsForRP()
        {
            try
            {
                var companyId = _companyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;
                var chartOfAccounts = await _chartOfAccountRepository.GetAll()
                    .Where(x => x.CompanyId == companyId && x.AccountNature == AccountNature.Asset && x.MainHeadId == 1) //1 is id for bank & cash main head
                    .ToListAsync();

                var chartOfAccountDtos = ObjectMapper.Map<List<ChartOfAccountDto>>(chartOfAccounts);
                return chartOfAccountDtos;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<ChartOfAccountDto>> GetChartOfAccountsForJV()
        {
            try
            {
                var companyId = _companyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;
                var chartOfAccounts = await _chartOfAccountRepository.GetAll()
                    .Where(x => x.CompanyId == companyId && x.AccountNature != AccountNature.Asset && x.AccountNature != AccountNature.Liabilities)
                    .ToListAsync();

                var chartOfAccountDtos = ObjectMapper.Map<List<ChartOfAccountDto>>(chartOfAccounts);
                return chartOfAccountDtos;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<ChartOfAccountDto>> GetControlAccounts(EntityDto input)
        {
            var chartOfAccounts = await _chartOfAccountRepository.GetAll()
                .Where(x => x.AccountTypeId == input.Id && x.AccountStatus == false && x.CompanyId == AbpSession.TenantId)
                .ToListAsync();

            var chartOfAccountDtos = ObjectMapper.Map<List<ChartOfAccountDto>>(chartOfAccounts);
            return chartOfAccountDtos;
        }
        public async Task<List<ChartOfAccountDto>> GetAll()
        {
            var filteredQuery = _chartOfAccountRepository.GetAll();
            var companyId = _companyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;
            var data = from o in filteredQuery
                       where o.CompanyId == companyId
                       select new ChartOfAccountDto
                       {
                           Id = o.Id,
                           AccountType = o.AccountType == null ? null : o.AccountType.Name,
                           AccountDescription = o.AccountDescription,
                           AccountCode = o.AccountCode,
                           AccountTypeId = o.AccountTypeId,
                           AccountTypeCode = _accountTypesRepository.GetAll().Where(x => x.Id == o.AccountTypeId).FirstOrDefault().Code,
                           MainHeadId = o.MainHeadId,
                           IsActive = o.IsActive,
                           AccountNature = o.AccountNature == 0 ? "Asset" : (int)o.AccountNature == 3 ? "Expenses" : (int)o.AccountNature == 2 ? "Income" : "Liabilities",
                           AccountNatureId = (int)o.AccountNature,
                           MainHead = o.MainHead == null ? null : o.MainHead.Name,
                           MainHeadCode = o.MainHead == null ? null : o.MainHead.Code
                       };

            return await data.ToListAsync();
        }

        public async Task<PagedResultDto<ChartOfAccountListViewDto>> GetAllChartOfAccountPagedList(PagedUserResultRequestDto input)
        {
            var data = _chartOfAccountRepository.GetAll()
                   .GroupBy(x => new
                   {
                       x.MainHead.Id,
                       x.MainHead.Name,
                       x.AccountNature,
                   })
                   .Select(t => new ChartOfAccountListViewDto
                   {
                       MainHeadId = t.Key.Id,
                       MainHead = t.Key.Name,
                       AccountNature = t.Key.AccountNature == 0 ? "Asset" : (int)t.Key.AccountNature == 3 ? "Expenses" : (int)t.Key.AccountNature == 2 ? "Income" : "Liabilities",
                       AccountNatureId = (int)t.Key.AccountNature,
                       //AccountTypeId = t.Key.AccountType.Id,
                       //AccountType = t.Key.AccountType.Name,
                       RelevantAccounts = t.Select(o => new ChartOfAccountListViewDto
                       {
                           Id = o.Id,
                           AccountType = o.AccountType == null ? null : o.AccountType.Name,
                           AccountDescription = o.AccountDescription,
                           AccountTypeId = o.AccountTypeId,
                           MainHeadId = o.MainHeadId,
                           IsActive = o.IsActive,
                           AccountNature = o.AccountNature == 0 ? "Asset" : (int)o.AccountNature == 3 ? "Expenses" : (int)o.AccountNature == 2 ? "Income" : "Liabilities",
                           MainHead = o.MainHead == null ? null : o.MainHead.Name

                       }).ToList()
                   });

            var totalCount = await data.CountAsync();

            var sortInput = input as ISortedResultRequest;
            var pagedData = await data.OrderBy(sortInput?.Sorting ?? "mainHeadId desc").PageBy(input).ToListAsync();

            return new PagedResultDto<ChartOfAccountListViewDto>(totalCount, pagedData);
        }

        public async Task<List<MainHeadDto>> GatAllMainHeadByAccountType(int accountTypeId)
        {
            var filteredMainHead = _mainHeadRepository.GetAll().Where(x => x.AccountTypeId == accountTypeId);
            var data = from o in filteredMainHead
                       select new MainHeadDto
                       {
                           Name = o.Name,
                           Id = o.Id,
                       };
            return await data.ToListAsync();
        }

        public async Task<List<MainHeadDto>> GetAllMainHead()
        {
            var filteredMainHead = _mainHeadRepository.GetAll().ToList();
            var data = from o in filteredMainHead
                       select new MainHeadDto
                       {
                           Name = o.Name,
                           Id = o.Id,
                       };
            return data.ToList();
        }

        public async Task<List<AccountTypeDto>> GetAllAccountTypeByAccountNature(int accountNatureId)
        {
            var filteredAccountTypes = _accountTypesRepository.GetAll().Where(x => x.AccountNature == (AccountNature)accountNatureId);
            var data = from o in filteredAccountTypes
                       select new AccountTypeDto
                       {
                           Name = o.Name,
                           Id = o.Id,
                       };
            return await data.ToListAsync();
        }

        public async Task<long> GetLastAccountTypeCodeByNature(int accountNature)
        {
            try
            {
                var accountNatureObj = (AccountNature)accountNature;

                if (_accountTypesRepository.GetAll().Count() > 0)
                {

                    return _accountTypesRepository.GetAll().Where(x => x.AccountNature == accountNatureObj).Count() > 0 ? _accountTypesRepository.GetAll().Where(x => x.AccountNature == accountNatureObj).OrderByDescending(x => x.Id).FirstOrDefault().Code : 0;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<long> GetLastMainHeadByAccountType(int accountTypeId)
        {
            try
            {
                if (_mainHeadRepository.GetAll().Count() > 0)
                {
                    return _mainHeadRepository.GetAll().Where(x => x.AccountTypeId == accountTypeId).Count() > 0 ? _mainHeadRepository.GetAll().Where(x => x.AccountTypeId == accountTypeId).OrderByDescending(x => x.Id).FirstOrDefault().Code : 0;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<IEnumerable<ChartOfAccountDto>> GetAllLiabilitiesAccount()
        {
            try
            {
                var list = await _chartOfAccountRepository.GetAllListAsync(x => x.AccountNature == AccountNature.Liabilities);
                var coaList = from data in list
                              select new ChartOfAccountDto
                              {
                                  Id = data.Id,
                                  AccountType = data.AccountType == null ? null : data.AccountType.Name,
                                  AccountDescription = data.AccountDescription,
                                  AccountTypeId = data.AccountTypeId,
                                  MainHeadId = data.MainHeadId,
                                  IsActive = data.IsActive,
                                  AccountNature = (int)data.AccountNature == 3 ? "Income" : null,
                                  MainHead = data.MainHead == null ? null : data.MainHead.Name
                              };

                return coaList;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<IEnumerable<ChartOfAccountDto>> GetAllAdvanceSaleTaxAccount()
        {
            try
            {
                var list = await _chartOfAccountRepository.GetAllListAsync(x => x.AccountNature == AccountNature.Asset && x.AccountTypeId != 3 && x.AccountTypeId != 1); //3 is for account receivables

                var coaList = from data in list
                              select new ChartOfAccountDto
                              {
                                  Id = data.Id,
                                  AccountType = data.AccountType == null ? null : data.AccountType.Name,
                                  AccountDescription = data.AccountDescription,
                                  AccountTypeId = data.AccountTypeId,
                                  MainHeadId = data.MainHeadId,
                                  IsActive = data.IsActive,
                                  AccountNature = (int)data.AccountNature == 3 ? "Income" : null,
                                  MainHead = data.MainHead == null ? null : data.MainHead.Name
                              };

                return coaList;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
