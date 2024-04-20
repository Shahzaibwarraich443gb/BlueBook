using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using AccountingBlueBook.AppServices.Invoices;
using AccountingBlueBook.AppServices.Invoices.dto;
using AccountingBlueBook.Authorization.Users;
using AccountingBlueBook.Common.CommonLookupDto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using AccountingBlueBook.Entities.MainEntities.LinkedAccounts;
using AccountingBlueBook.Entities.MainEntities.Vouchers;
using AccountingBlueBook.Entities.NormalizeEntities;
using AccountingBlueBook.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccountingBlueBook.AppServices.Estimate
{
    [AbpAuthorize]
    public class EstimateAppService : AccountingBlueBookAppServiceBase, IInvoiceAppService
    {
        private readonly IRepository<ProductService> _productServiceRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Voucher, long> _voucherRepository;
        private int tenantId = 0;
        private readonly IRepository<LinkedAccount, long> _linkedAccountRepository;
        private readonly IRepository<VoucherDetail, long> _voucherDetailRepository;
        private readonly IRepository<InvoiceDetail, long> _invoiceDetailRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly InvoiceAppService _invoiceAppService;
        private readonly IRepository<PaymentTermList,long> _paymentTermListRepository;
        
        
        public EstimateAppService(IRepository<ProductService> productServiceRepository,
                                 IRepository<Customer> customerRepository,
                                 IRepository<Voucher, long> voucherRepository,
                                 IRepository<VoucherDetail, long> voucherDetailRepository,
                                 IRepository<LinkedAccount, long> linkedAccountRepository,
                                 IInvoiceRepository invoiceRepository,
                                 IRepository<InvoiceDetail, long> invoiceDetailRepository,
                                 IRepository<Company> companyRepository,
                                 InvoiceAppService invoiceAppService,
                                 IRepository<PaymentTermList,long> paymentTermListRepository)
        {
            _productServiceRepository = productServiceRepository;
            _customerRepository = customerRepository;
            _voucherRepository = voucherRepository;
            _linkedAccountRepository = linkedAccountRepository;
            _voucherDetailRepository = voucherDetailRepository;
            _invoiceDetailRepository = invoiceDetailRepository;
            _invoiceRepository = invoiceRepository;
            _companyRepository = companyRepository;
            _invoiceAppService = invoiceAppService;
            _paymentTermListRepository = paymentTermListRepository;
        }

        public async Task<InvoiceDto> Get(EntityDto<long> input)
        {
            var invoice = await _invoiceRepository.GetAll()
                                                  .Include(a => a.InvoiceDetails)
                                                  .FirstOrDefaultAsync();

            if (invoice == null) throw new UserFriendlyException("Invoice not found");
            var invoicedto = ObjectMapper.Map<InvoiceDto>(invoice);
            return invoicedto;

        }


        public async Task SaveInvoice(CreateInvoiceDto input)
        {
            try
            {

                var customer = await _customerRepository.FirstOrDefaultAsync(x => x.Id == input.Invoice.RefCustomerID);
                Invoice invoice = new Invoice
                {
                    EstimateDate = input.Invoice.EstimateDate,
                    ExpirationDate = input.Invoice.ExpirationDate,
                    RefCustomerId = customer.Id,
                    RefCompanyId = AbpSession.TenantId != null ? (int)AbpSession.TenantId : tenantId,
                    Total = input.Invoice.Total,
                    Email = input.Invoice.Email.Count() > 0 ? string.Join(",", input.Invoice.Email) : null,
                    //InvoiceType = (InvoiceType)input.Invoice.RefInvoiceType,
                    InvoiceStatus = InvoiceStatus.Paid,
                    InvoiceType = InvoiceType.Estimate,
                    IsPaid = false,
                    InvoiceNo = input.Invoice.InvoiceNo,
                    Note = input.Invoice.Note,
                };

                List<InvoiceDetail> invoiceDetailList = input.Invoice.InvoiceDetails.Select(data => new InvoiceDetail
                {
                  
                    Id = data.Id == null ? 0 : (long)data.Id,
                    InvoiceId = input.Invoice.InvoiceId == null ? 0 : (long)input.Invoice.InvoiceId,
                    Amount = data.Amount,
                    Discount = data.Discount,
                    Quantity = data.Quantity,
                    Rate = data.Rate,
                    RefProducId = data.RefProducID,
                    SaleTax = data.SaleTax,
                    Description = data.Description,
                    RefCustomerId = input.Invoice.Id
                }).ToList();
                invoice.InvoiceDetails = invoiceDetailList;
                if (invoice.Id > 0)
                {
                    await _invoiceAppService.UpdateInvoiceDetails(invoice);
                }
                else
                {
                    await _invoiceAppService.CreateInvoice(invoice);
                }
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        


        public async Task<List<PaymentTermList>>GetPaymentTermTermLists()

        {
            try
            {
                List<PaymentTermList> model = new List<PaymentTermList>();
               var  modelDbList =await _paymentTermListRepository.GetAll().Select(x=> new PaymentTermList
               { Days = x.Days, Description = x.Description, Id = x.Id }).ToListAsync();
             
                return modelDbList;
            }
            catch (Exception e)
            {
               // ExceptionHandler.LogMessage(e, "BL_Invoice, Get_Payment_Term_List");
                return null;
            }
        }
        public Task<List<ProductService>> GetActiveProductList()
        {
            return null;
        }
       
      

        //To do Email working is pending
        public async void EmailEstimate(EntityDto input, string to, string Emails = "")
        {
            try
            {
               
                if (input.Id > 0)
                {
                    //to do:  get estimate data to create email body
                    //to do: needs to send email
                    var _Invoice = _invoiceAppService.GetInvoiceAsync(input.Id);
                   
                    if (_Invoice !=null)
                    {
                        var customer = await _customerRepository.GetAsync((int)_Invoice.Result.RefCustomerId);

                        var invoiceDetail = await _invoiceDetailRepository.GetAll().Where(x => x.InvoiceId == input.Id).ToListAsync();
                        var company = await GetCompanyName((int)_Invoice.Result.RefCompanyId);
                        var balance = _invoiceAppService.GetInvoiceBalance(_Invoice.Id);
                        _Invoice.Result.PaidAmount=Convert.ToDecimal( balance);
                        List<PaymentTermList> _PaymentTermList = await GetPaymentTermTermLists();
                        List<ProductService> _Products = _productServiceRepository.GetAll().Select(x => new ProductService { Name = x.Name, Id = x.Id, SalePrice = (x.SalePrice == null ? 0 : x.SalePrice), SaleTax = x.SaleTax }).ToList();
                        List<Customer> _Cusotmers = _customerRepository.GetAll().Select(x => new Customer { FirstName = x.FirstName, LastName = x.LastName, CustomerTypeId = x.CustomerTypeId, Email = x.Email, ContactInfo = x.ContactInfo, Address = x.Address }).ToList();
                        var combineDto = new EstimateCombineDto();
                        combineDto.Customer = customer;
                        combineDto.InvoiceDetail = invoiceDetail;
                        combineDto.Company = company;
                        combineDto.Balance =Convert.ToDecimal(balance);

                    }

                }

            }
            catch (Exception ex)
            {
                

            }
        }
        private async Task CreateInvoice(Invoice invoice, Voucher voucher)
        {
            try
            {
                invoice.IsActive = true;
                invoice.IsPaid = true;

                await _invoiceAppService.CreateInvoice(invoice);

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("An error occurred while creating invoice", ex.Message);
            }
        }

     

       

      


      


       

     


      

  

      


     


        public async Task<decimal> GetInvoiceBalance(long id)
        {
            try
            {

                decimal _InvoiceBalance = 0;
                string ResObjJSON = await _invoiceRepository.GetInvoiceBalance(id);

                var ResObjParsed = JsonConvert.DeserializeObject<dynamic>(ResObjJSON);

                if (ResObjParsed.Status == "Successful")
                {
                    return ResObjParsed.InvoiceBalance;
                }
                return _InvoiceBalance;
            }
            catch (Exception)
            {

                throw;
            }

            //to do: call this procedure "sp_Finance_Get_InvoiceBalance" to get invoice balance
            //done by dev-10
        }
        //Not involve in the Extimate 
        //private async Task<Voucher> AddSpecialVoucherDetails(Invoice invoice, Voucher voucher, long companyId, decimal discountGiven)
        //{
        //    var salesTaxPayableLinkedAccountId = await GetLinkedAccountId((int)LinkedAccountEnum.Sale_Tax_Payables, companyId);
        //    var customerReceivableLinkedAccountId = await GetLinkedAccountId((int)LinkedAccountEnum.Customers_Receivables, companyId);
        //    var discountsGivenLinkedAccountId = await GetLinkedAccountId((int)LinkedAccountEnum.Discount_Given, companyId);

        //    voucher.VoucherDetails.Add(new VoucherDetail()
        //    {
        //        RefCompanyId = 0,
        //        VoucherId = 0,
        //        RefChartOfAccountId = salesTaxPayableLinkedAccountId,
        //        SrNo = 1,
        //        Note = "",
        //        Dr_Amount = 0,
        //        BankId = 0,
        //        Cr_Amount = CalculateSaletax(invoice.InvoiceDetails.ToList()),
        //        AddDate = DateTime.Today,
        //        TransactionDate = DateTime.Today,
        //        IsDeleted = false,
        //        PartnerId = 0,
        //        InvoiceId = 0,
        //        AccountName = "",
        //    });

        //    voucher.VoucherDetails.Add(new VoucherDetail()
        //    {
        //        RefCompanyId = 0,
        //        VoucherId = 0,
        //        RefChartOfAccountId = customerReceivableLinkedAccountId,
        //        SrNo = 2,
        //        Note = "",
        //        Dr_Amount = invoice.Total,
        //        BankId = 0,
        //        Cr_Amount = 0,
        //        AddDate = DateTime.Today,
        //        TransactionDate = DateTime.Today,
        //        IsDeleted = false,
        //        PartnerId = invoice.RefCustomerId,
        //        InvoiceId = 0,
        //        AccountName = "",
        //    });

        //    voucher.VoucherDetails.Add(new VoucherDetail()
        //    {
        //        RefCompanyId = 0,
        //        VoucherId = 0,
        //        RefChartOfAccountId = discountsGivenLinkedAccountId,
        //        SrNo = 2,
        //        Note = "",
        //        Dr_Amount = discountGiven,
        //        BankId = 0,
        //        Cr_Amount = 0,
        //        AddDate = DateTime.Today,
        //        TransactionDate = DateTime.Today,
        //        IsDeleted = false,
        //        PartnerId = invoice.RefCustomerId,
        //        InvoiceId = 0,
        //        AccountName = "",
        //    });

        //    return voucher;

        //}

        private decimal CalculateSaletax(List<InvoiceDetail> list)
        {
            try
            {
                decimal TotalSaleTax = 0M;
                for (int i = 0; i < list.Count(); i++)
                {
                    decimal TotalRate = (list[i].Rate.HasValue ? list[i].Rate.Value : 0) * (list[i].Quantity.HasValue ? list[i].Quantity.Value : 0);
                    decimal TotalRateAfterDiscount = TotalRate - (TotalRate * (list[i].Discount.HasValue ? list[i].Discount.Value : 0) / 100);
                    TotalSaleTax += (TotalRateAfterDiscount * (list[i].SaleTax.HasValue ? list[i].SaleTax.Value : 0) / 100);
                }
                return TotalSaleTax;
            }
            catch (Exception ex)
            {
                Logger.Error("CalculateSaletax method failed in InvoiceAppService", ex);
                return 0;
            }
        }

        private async Task<long> GetLinkedAccountId(int id, long companyId)
        {
            var linkedAccount = await _linkedAccountRepository.GetAll()
                                                              .Where(a => a.Id == id && a.CompanyID == companyId)
                                                              .Select(a => a.Id)
                                                              .FirstOrDefaultAsync();
            if (linkedAccount == 0) throw new UserFriendlyException("linkedAccount not found");

            return linkedAccount;
        }

        public async Task<ListResultDto<KeyValuePair<int, string>>> ProductServicesLookUp(CommonLookupInput<string> input)
        {
            var products = await _productServiceRepository.GetAll()
                                .WhereIf(!input.Item.IsNullOrEmpty(), a => a.Name.StartsWith(input.Item))
                                .OrderBy(a => a.Name)
                                .Select(a => new KeyValuePair<int, string>(a.Id, a.Name))
                                .Skip(input.SkipCount)
                                .Take(input.MaxResultCount)
                                .ToListAsync();

            return new ListResultDto<KeyValuePair<int, string>>() { Items = products };
        }

        public async Task<ListResultDto<CustomerLookupOutput>> CustomersLookUp(CommonLookupInput<string> input)
        {
            var customers = await _customerRepository.GetAll()
                                .WhereIf(!input.Item.IsNullOrEmpty(), a => a.Name.StartsWith(input.Item))
                                .OrderBy(a => a.Name)
                                .Include(a => a.Company)
                                .Select(a => new CustomerLookupOutput()
                                {
                                    Id = a.Id,
                                    Name = a.Name,
                                    CompanyName = a.Company.Name ?? string.Empty
                                })
                                .Skip(input.SkipCount)
                                .Take(input.MaxResultCount)
                                .ToListAsync();

            return new ListResultDto<CustomerLookupOutput> { Items = customers };
        }

        public async Task<string> GetCompanyName(int customerId)
        {

            var customer = await _customerRepository.GetAsync(customerId);
            if (customer != null)
            {

                var company = await _companyRepository.GetAll().Where(x => x.TenantId == customer.TenantId).FirstOrDefaultAsync();
                if (company != null)
                {
                    return company.Name;
                }
            }
            return null;

        }

    }




}
