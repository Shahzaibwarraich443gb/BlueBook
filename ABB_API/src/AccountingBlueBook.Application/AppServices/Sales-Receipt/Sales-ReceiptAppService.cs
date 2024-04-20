using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;

using AccountingBlueBook.AppServices.Estimate;
using AccountingBlueBook.AppServices.Invoices.dto;
using AccountingBlueBook.AppServices.SalesReceipt.dto;
using AccountingBlueBook.Authorization.Users;
using AccountingBlueBook.Common.CommonLookupDto;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using AccountingBlueBook.Entities.MainEntities.LinkedAccounts;
using AccountingBlueBook.Entities.MainEntities.Vouchers;
using AccountingBlueBook.Entities.NormalizeEntities;
using AccountingBlueBook.Enums;
using AutoMapper.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AccountingBlueBook.AppServices.Invoices;
using iTextSharp.text;

namespace AccountingBlueBook.AppServices.SalesReceipt
{
    public class SalesReceiptAppService : AccountingBlueBookAppServiceBase, ISalesReceiptAppService
    {
        private readonly IRepository<ProductService> _productServiceRepository;
        private readonly IRepository<Customer> _customerRepository;
        private int tenantId = 0;
        private readonly IRepository<Voucher, long> _voucherRepository;
        private readonly IRepository<LinkedAccount, long> _linkedAccountRepository;
        private readonly IRepository<VoucherDetail, long> _voucherDetailRepository;
        private readonly IRepository<InvoiceDetail, long> _invoiceDetailRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly InvoiceAppService _invoiceService;
        private readonly EstimateAppService _estimateAppService;

        public SalesReceiptAppService(IRepository<ProductService> productServiceRepository,
                                 IRepository<Customer> customerRepository,
                                 IRepository<Voucher, long> voucherRepository,
                                 IRepository<VoucherDetail, long> voucherDetailRepository,
                                 IRepository<LinkedAccount, long> linkedAccountRepository,
                                 IInvoiceRepository invoiceRepository,
                                  InvoiceAppService invoiceService,
                                 IRepository<InvoiceDetail, long> invoiceDetailRepository,
                                 EstimateAppService estimateAppService)
        {
            _productServiceRepository = productServiceRepository;
            _customerRepository = customerRepository;
            _voucherRepository = voucherRepository;
            _invoiceService = invoiceService;
            _linkedAccountRepository = linkedAccountRepository;
            _voucherDetailRepository = voucherDetailRepository;
            _invoiceDetailRepository = invoiceDetailRepository;
            _invoiceRepository = invoiceRepository;
            _estimateAppService = estimateAppService;
        }

        public async Task<InvoiceDto> Get(EntityDto<long> input)
        {
            var invoicedto = new InvoiceDto();
            if (input.Id > 0)
            {
                var invoice = await _invoiceRepository.GetAll()
                                            .WhereIf(input.Id > 0, a => a.Id == input.Id)
                                            .Include(a => a.InvoiceDetails)
                                            .FirstOrDefaultAsync();

                if (invoice == null) throw new UserFriendlyException("Invoice not found");

                invoicedto = ObjectMapper.Map<InvoiceDto>(invoice);
         
            }
            return invoicedto;

        }


        public async Task SaveInvoice(CreateSalesReceiptDto input)
        {
            try
            {

                //ObjectMapper.Map<Invoice>(input.Invoice);
                var customer = await _customerRepository.FirstOrDefaultAsync(x => x.Id == input.SalesReceipt.RefCustomerID);
                Invoice invoice = new Invoice

                {

                    Id = input.SalesReceipt.InvoiceId == null ? 0 : (long)input.SalesReceipt.InvoiceId,
                    SaleReceiptDate = input.SalesReceipt.SaleReceiptDate,
                    RefrenceNo = input.SalesReceipt.RefrenceNo,
                    RefPaymentMethodId = input.SalesReceipt.RefPaymentMethodID,
                    RefDepositToAccountId = input.SalesReceipt.RefDepositToAccountID,
                    RefCustomerId = customer.Id,
                    Email = input.SalesReceipt.Email.Count() > 0 ? string.Join(",", input.SalesReceipt.Email) : null,
                    InvoiceType = InvoiceType.Sale_Receipt,
                    TenantId = AbpSession.TenantId == null ? 1 : (int)AbpSession.TenantId,
                    RefCompanyId = AbpSession.TenantId == null ? 1 : (int)AbpSession.TenantId,
                    InvoiceNo = input.SalesReceipt.InvoiceNo,
                    InvoiceStatus = InvoiceStatus.Paid,
                    IsPaid = true,
                    IsActive = true,
                    Total = input.SalesReceipt.Total,
                     IsSendLater = input.SalesReceipt.IsSendLater,
                     



                };

                List<InvoiceDetail> invoiceDetailList = input.SalesReceipt.InvoiceDetails.Select(data => new InvoiceDetail
                {
                    
                    
                    Id = data.Id == null ? 0 : (long)data.Id,
                    TenantId = AbpSession.TenantId != null ? (int)AbpSession.TenantId : tenantId,
                    InvoiceId = input.SalesReceipt.InvoiceId == null ? 0 : (long)input.SalesReceipt.InvoiceId,
                    Amount = data.Amount,
                    Discount = data.Discount,
                    Quantity = data.Quantity,
                    Rate = data.Rate,
                    RefProducId = data.RefProducID,
                    SaleTax = data.SaleTax,
                   
                   
                    PaidAmount = data.PaidAmount
                }).ToList();

                invoice.InvoiceDetails = invoiceDetailList;
                List<Voucher> Voucher = new List<Voucher>();

                Voucher _SaleVoucherMaster = new Voucher()
                {

                    VoucherNo = "",
                    VoucherTypeCode = "SV",
                    AddDate = DateTime.Today,
                    TransactionDate = DateTime.Today,
                    PaymentMode = 0,
                    CreatorUserId = 0,
                    LastModifierUserId = 0,
                    IsDeleted = false,
                    LastModificationTime = DateTime.Today,
                    PaymentType = 0,
                };
                foreach (var item in invoiceDetailList)
                {
                    //long? productid = item.ref_ProducID;
                    //long? IncomeAccountId = new BL_Products().Get_Active_Products_List().Where(x => x.ProductID == productid).FirstOrDefault().IncomeAccId;
                    VoucherDetail _SaleVoucherDetailCreditEntry = new VoucherDetail()
                    {
                        RefCompanyId = 0,
                        VoucherId = 0,
                        //ref_ChartOfAccountID = IncomeAccountId == 0 ? (int)eLinkedAccounts.Sales_Revenue : IncomeAccountId,
                        SrNo = 1,
                        Note = "",
                        Dr_Amount = 0,
                        BankId = 0,
                        Cr_Amount = ((item.Rate.HasValue ? item.Rate.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0)) - (((item.Rate.HasValue ? item.Rate.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0) * (item.Discount.HasValue ? item.Discount.Value : 0)) / 100),
                        AddDate = DateTime.Today,
                        TransactionDate = DateTime.Today,
                        //ModifiedDate = DateTime.Today,
                        //AddedByUserID = 1,
                        //ModifiedByUserID = 1,
                        IsDeleted = false,
                        PartnerId = 0,
                        InvoiceId = 0,
                        AccountName = "",
                    };
                    _SaleVoucherMaster.VoucherDetails.Add(_SaleVoucherDetailCreditEntry);

                }
                VoucherDetail _SaleVoucherDetailSaletaxCreditEntry = new VoucherDetail()
                {
                    RefCompanyId = 0,
                    VoucherId = 0,
                    //ref_ChartOfAccountID = new BL_Invoice().Get_Linked_AccountID((int)eLinkedAccounts.Sale_Tax_Payables),
                    SrNo = 1,
                    Note = "",
                    Dr_Amount = 0,
                    BankId = 0,
                    Cr_Amount = CalculateSaletax(invoiceDetailList),
                    AddDate = DateTime.Today,
                    TransactionDate = DateTime.Today,
                    //ModifiedDate = DateTime.Today,
                    //AddedByUserID = 1,
                    //ModifiedByUserID = 1,
                    IsDeleted = false,
                    PartnerId = 0,
                    InvoiceId = 0,
                    AccountName = "",
                };
                VoucherDetail _SaleVoucherDetailDebitEntry = new VoucherDetail()
                {

                    RefCompanyId = 0,
                    VoucherId = 0,
                    //ref_ChartOfAccountID = new BL_Invoice().Get_Linked_AccountID((int)eLinkedAccounts.Customers_Receivables),
                    SrNo = 2,
                    Note = "",
                    Dr_Amount = invoice.Total,
                    BankId = 0,
                    Cr_Amount = 0,
                    AddDate = DateTime.Today,
                    TransactionDate = DateTime.Today,
                    //ModifiedDate = DateTime.Today,
                    //AddedByUserID = 1,
                    //ModifiedByUserID = 1,
                    IsDeleted = false,
                    PartnerId = invoice.RefCustomerId,
                    InvoiceId = 0,
                    AccountName = "",

                };
                VoucherDetail _DiscountGivenDetailDebitEntry = new VoucherDetail()
                {
                    RefCompanyId = 0,
                    VoucherId = 0,
                    //ref_ChartOfAccountID = new BL_Invoice().Get_Linked_AccountID((int)eLinkedAccounts.Discount_Given),
                    SrNo = 2,
                    Note = "",
                    Dr_Amount = invoiceDetailList.Sum(m => m.Rate).Value - _SaleVoucherMaster.VoucherDetails.Sum(m => m.Cr_Amount).Value,
                    BankId = 0,
                    Cr_Amount = 0,
                    AddDate = DateTime.Today,
                    TransactionDate = DateTime.Today,
                    //ModifiedDate = DateTime.Today,
                    //AddedByUserID = 1,
                    //ModifiedByUserID = 1,
                    IsDeleted = false,
                    PartnerId = invoice.RefCustomerId,
                    InvoiceId = 0,
                    AccountName = "",
                };
                _SaleVoucherMaster.VoucherDetails.Add(_DiscountGivenDetailDebitEntry);
                _SaleVoucherMaster.VoucherDetails.Add(_SaleVoucherDetailSaletaxCreditEntry);
                _SaleVoucherMaster.VoucherDetails.Add(_SaleVoucherDetailDebitEntry);

                Voucher _PaymentVoucherMaster = new Voucher()
                {
                    VoucherNo = "",
                    VoucherTypeCode = "CR",
                    AddDate = DateTime.Today,
                    TransactionDate = DateTime.Today,
                    //ModifyDate = DateTime.Today,
                    //AddedByUserID = 0,
                    //ModifiedByUserID = 0,
                    PaymentMode = 0,
                    IsDeleted = false,
                    PaymentType = 0,

                };
                VoucherDetail _PaymentVoucherDetailCreditEntry = new VoucherDetail()
                {
                    RefCompanyId = 0,
                    VoucherId = 0,
                    //ref_ChartOfAccountID = new BL_Invoice().Get_Linked_AccountID((int)eLinkedAccounts.Customers_Receivables),
                    SrNo = 1,
                    Note = "",
                    Dr_Amount = 0,
                    BankId = 0,
                    Cr_Amount = invoice.Total,
                    AddDate = DateTime.Today,
                    TransactionDate = DateTime.Today,
                    //ModifiedDate = DateTime.Today,
                    //AddedByUserID = 1,
                    //ModifiedByUserID = 1,
                    IsDeleted = false,
                    PartnerId = invoice.RefCustomerId,
                    InvoiceId = 0,
                    AccountName = "",
                };
                VoucherDetail _PaymentVoucherDetailDebitEntry = new VoucherDetail()
                {

                    RefCompanyId = 0,
                    VoucherId = 0,
                    RefChartOfAccountId = invoice.RefDepositToAccountId,
                    SrNo = 2,
                    Note = "",
                    Dr_Amount = invoice.Total,
                    BankId = 0,
                    Cr_Amount = 0,
                    AddDate = DateTime.Today,
                    TransactionDate = DateTime.Today,
                    //ModifiedDate = DateTime.Today,
                    //AddedByUserID = 1,
                    //ModifiedByUserID = 1,
                    IsDeleted = false,
                    PartnerId = 0,
                    InvoiceId = 0,
                    AccountName = "",

                };
                _PaymentVoucherMaster.VoucherDetails.Add(_PaymentVoucherDetailCreditEntry);
                _PaymentVoucherMaster.VoucherDetails.Add(_PaymentVoucherDetailDebitEntry);
                Voucher.Add(_SaleVoucherMaster);
                Voucher.Add(_PaymentVoucherMaster);

                if (invoice.Id > 0)
                {
                    await _invoiceService.UpdateInvoiceDetails(invoice);
                }


                else
                {
                    long invoiceId = await _invoiceService.CreateSRInvoice(invoice, Voucher);
                    long voicherId = await _invoiceService.AddNewVouchersSaleReceipt(invoiceId, Voucher);
                 
                }

            }

            catch (Exception ex)
            {
                throw;
            }


        }
  

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
        private decimal CalculateSaleRevenue(List<InvoiceDetail> list)
        {
            try
            {
                decimal TotalAmountAfterDiscount = 0M;
                for (int i = 0; i < list.Count(); i++)
                {
                    decimal TotalAmount = (list[i].Rate.HasValue ? list[i].Rate.Value : 0) * (list[i].Quantity.HasValue ? list[i].Quantity.Value : 0);
                     TotalAmountAfterDiscount = TotalAmount - (TotalAmount * (list[i].Discount.HasValue ? list[i].Discount.Value : 0) / 100);
                }
                return TotalAmountAfterDiscount;
            }
            catch (Exception ex)
            {
                Logger.Error("CalculateSaletax method failed in InvoiceAppService", ex);
                return 0;
            }
        }

        private async Task<long> GetLinkedAccountId(int id, long companyId)
        {
            var linkedAccount = await _linkedAccountRepository.GetAll().Where(a => a.Id == id && a.CompanyID == companyId).Select(a => a.Id).FirstOrDefaultAsync();
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

      

    }




}
