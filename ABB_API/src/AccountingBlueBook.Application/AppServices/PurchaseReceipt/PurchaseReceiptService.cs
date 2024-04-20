using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AccountingBlueBook.AppServices.Invoices;
using AccountingBlueBook.AppServices.PurchaseReceipt.Dto;
using AccountingBlueBook.AppServices.ReceivedPayment;
using AccountingBlueBook.AppServices.ReceivedPayment.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using AccountingBlueBook.Entities.MainEntities.LinkedAccounts;
using AccountingBlueBook.Entities.MainEntities.Vouchers;
using AccountingBlueBook.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.PurchaseReceipt
{
    [AbpAuthorize]
    public class PurchaseReceiptService : AccountingBlueBookAppServiceBase, IPurchaseReceiptService
    {
        private readonly IRepository<ProductService> _productServiceRepository;
        private readonly IRepository<ChartOfAccount> _chartOfAccountRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Merchant> _merchantRepository;
        private readonly IRepository<Customer> _customerRepository;
        private static string ENDPOINT = "";
        private readonly IRepository<CardTransaction> _cardTransactionRepository;
        private readonly IRepository<Invoice, long> _invoiceRepository;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<LinkedAccount, long> _linkedAccountRepository;
        private readonly InvoiceAppService _invoiceService;
        private readonly IRepository<InvoiceDetail, long> _invoiceDetailRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly ReceivedPaymentService _receivePaymentService;
        private readonly IRepository<Voucher, long> _voucherRepository;
        private readonly IRepository<VoucherDetail, long> _voucherDetailRepository;
        



        //private readonly IEmailAppServices _emailAppService;
        private enum OPERATIONS { GET, PUT, POST, DELETE };
        public PurchaseReceiptService(IRepository<Transaction> transactionRepository, IRepository<CardTransaction> cardTransactionRepository,
            IRepository<Customer> customerRepository, IRepository<Invoice, long> invoiceRepository, InvoiceAppService invoiceService, IRepository<Company> companyRepository,
            IRepository<Merchant> merchantRepository, IRepository<InvoiceDetail, long> invoiceDetailRepository, IRepository<Vendor> vendorRepository, IRepository<Voucher, long> voucherRepository,
             IRepository<LinkedAccount, long> linkedAccountRepository,
             IRepository<ProductService> productServiceRepository,
              ReceivedPaymentService receivePaymentService,
              IRepository<VoucherDetail, long> voucherDetailRepository,
             IRepository<ChartOfAccount> chartOfAccountRepository

            //, IEmailAppServices emailAppService
            )
        {
            //_emailAppService = emailAppService;
            _invoiceDetailRepository = invoiceDetailRepository;
            _customerRepository = customerRepository;
            _merchantRepository = merchantRepository;
            _companyRepository = companyRepository;
            _cardTransactionRepository = cardTransactionRepository;
            _invoiceRepository = invoiceRepository;
            _transactionRepository = transactionRepository;
            _invoiceService = invoiceService;
            _vendorRepository = vendorRepository;
            _linkedAccountRepository = linkedAccountRepository;
            _productServiceRepository = productServiceRepository;
            _receivePaymentService = receivePaymentService;
            _voucherRepository = voucherRepository;
            _voucherDetailRepository = voucherDetailRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
        }

        public async Task SavePurchaseReceipt(SavePurchaseReceiptDto input)
        {
            try
            {
                var supplier = await _vendorRepository.FirstOrDefaultAsync(x => x.Id == input.RefSupplierId);
                ChargeCardDto card_obj = new ChargeCardDto();
                if (input.ChargeCard != null)
                 {
                    if (!String.IsNullOrWhiteSpace(input.ChargeCard.CardNumber))
                    {
                        input.ChargeCard.Amount = input.Total;
                        //input.ChargeCard.CustomerEmail = customer.Email;
                        var IsAmountCharged = await _receivePaymentService.SaveChargeCard(input.ChargeCard);
                        if (IsAmountCharged == true)
                        {
                            await InvoiceDetails(input, supplier);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        await InvoiceDetails(input, supplier);
                    }
                }
                else
                {
                    await InvoiceDetails(input, supplier);
                }

            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred while creating payment received", ex.Message);
            };
        }

        public async Task  InvoiceDetails(SavePurchaseReceiptDto input, Vendor supplier)
        {
            var invoice = new Invoice
            {
                Id = input.Id == null ? 0 : (long)input.Id,
                TenantId = (int)AbpSession.TenantId,
                RefPaymentMethodId = input.RefPaymentMethodID,
                RefDepositToAccountId = input.RefDepositToAccountId,
                PurchaseReceiptDate = input.PurchaseReceiptDate,
                RefrenceNo = input.ReferenceNo,
                RefSupplierId=supplier.Id,
                IsSendLater=false,
                RefCashEquivalentsAccountId=input.RefCashEquivalentsAccountId,
                Total = input.Total,
                InvoiceType = (InvoiceType)input.RefInvoiceType,
                Note = input.Note,
                InvoiceGroupId = (int?)input.InvoiceID,
                InvoiceNo = input.InvoiceNo,
                CheckNo = input.CheckNo
            
               
            };
            List<InvoiceDetail> List = input.puchaseReceiptDto.Select(data => new InvoiceDetail
            {
                    Amount = data.Amount,
                    Discount = data.Discount,
                    Quantity = data.Quantity,
                    Rate = data.Rate,
                    RefProducId = data.RefProducID,
                    SaleTax = data.SaleTax,
                    Description=data.Description,
               
                    
            }).ToList();

            invoice.InvoiceDetails = List;
            var tenatId=(int)AbpSession.TenantId;
            var companyId = await GetCompanyId(tenatId);
                


            decimal discountGiven = 0;
            var productIds = input.puchaseReceiptDto.Select(a => a.RefProducID);
            List<Voucher> Voucher = new List<Voucher>();
            //var voucher = new Voucher();
            Voucher _PurchaseVoucherMaster = new Voucher()
            {

                VoucherNo = "",
                VoucherTypeCode = "PV",
                AddDate = DateTime.Today,
                TransactionDate = DateTime.Today,
                PaymentMode = 0,
                IsDeleted = false,
                PaymentType = 0,
            };
            //var salesVoucherMaster = new Voucher();
            //salesVoucherMaster = await _invoiceRepository.AddSpecialVoucherDetails(invoice, Voucher, companyId, discountGiven);

            VoucherDetail _PurchaseVoucherDetailCreditEntry = new VoucherDetail()
            {
                RefCompanyId = 0,
                VoucherId = 0,
                RefChartOfAccountId = invoice.RefCashEquivalentsAccountId,
                SrNo = 1,
                Note = "",
                Dr_Amount = 0,
                BankId = 0,
                Cr_Amount = invoice.Total,
                AddDate = DateTime.Today,
                TransactionDate = DateTime.Today,
                IsDeleted = false,
                PartnerId = 0,
                InvoiceId = 0,
                AccountName = "",
            };

            var vendorPayableLinkedAccountId = await GetCOA_Id("Vendor Payable", companyId);
            // var vendorPayableLinkedAccountId = await GetLinkedAccountId((int)LinkedAccountEnum.Vendor_Payables, companyId);

            VoucherDetail _PurchaseVoucherDetailDebitEntry = new VoucherDetail()
            {

                RefCompanyId = 0,
                VoucherId = 0,
                RefChartOfAccountId = vendorPayableLinkedAccountId,
                SrNo = 2,
                Note = "",
                Dr_Amount = invoice.Total,
                BankId = 0,
                Cr_Amount = 0,
                AddDate = DateTime.Today,
                TransactionDate = DateTime.Today,
                IsDeleted = false,
                PartnerId = invoice.RefSupplierId,
                InvoiceId = 0,
                AccountName = "",

            };
       
            _PurchaseVoucherMaster.VoucherDetails.Add(_PurchaseVoucherDetailCreditEntry);
            _PurchaseVoucherMaster.VoucherDetails.Add(_PurchaseVoucherDetailDebitEntry);



            Voucher _PaymentVoucherMaster = new Voucher()
            {
                VoucherNo = "",
                VoucherTypeCode = "EV",
                AddDate = DateTime.Today,
                TransactionDate = DateTime.Today,
              
                PaymentMode = 0,
                IsDeleted = false,
                PaymentType = 0,

            };

            VoucherDetail _PaymentVoucherDetailCreditEntry = new VoucherDetail()
            {
                RefCompanyId = 0,
                VoucherId = 0,
                RefChartOfAccountId = vendorPayableLinkedAccountId,
                SrNo = 1,
                Note = "",
                Dr_Amount = 0,
                BankId = 0,
                Cr_Amount = invoice.Total,
                AddDate = DateTime.Today,
                TransactionDate = DateTime.Today,
                
                IsDeleted = false,
                PartnerId = invoice.RefSupplierId,
                InvoiceId = 0,
                AccountName = "",
            };
            decimal DiscountTaken = 0;
           
            foreach (var item in List)
            {

                long? productid = item.RefProducId;
                var linkedAccountId = await GetLinkedAccountId((int)LinkedAccountEnum.Purchases, companyId);
                var ExpenseAccountId = await _productServiceRepository.GetAll().Where(a => productIds.Any(x => x == a.Id)).Select(a => a.Id).ToListAsync();
                VoucherDetail _PaymentVoucherDetailDebitEntry = new VoucherDetail()
                {
                    RefCompanyId = 0,
                    VoucherId = 0,
                    RefChartOfAccountId = ExpenseAccountId.Count(a => a == item.RefProducId) > 0 ? item.RefProducId : linkedAccountId,
                    SrNo = 2,
                    Note = "",
                    Dr_Amount = ((item.Rate.HasValue ? item.Rate.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0)) - (((item.Rate.HasValue ? item.Rate.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0) * (item.Discount.HasValue ? item.Discount.Value : 0)) / 100),
                    BankId = 0,
                    Cr_Amount = 0,
                    AddDate = DateTime.Today,
                    TransactionDate = DateTime.Today,
                    IsDeleted = false,
                    PartnerId = 0,
                    InvoiceId = 0,
                    AccountName = "",
                };

                decimal TotalAmount = (item.Rate.HasValue ? item.Rate.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0);
                DiscountTaken = DiscountTaken + ((TotalAmount * (item.Discount.HasValue ? item.Discount.Value : 0)) / 100);
                _PaymentVoucherMaster.VoucherDetails.Add(_PaymentVoucherDetailDebitEntry);
            }

            VoucherDetail _PaymentVoucherDetailSaleTaxDebitEntry = new VoucherDetail()
            {

                RefCompanyId = 0,
                VoucherId = 0,
                RefChartOfAccountId=await GetLinkedAccountId((int)LinkedAccountEnum.Sale_Tax_Paid,companyId ),
                SrNo = 2,
                Note = "",
                Dr_Amount = CalculateSaletax(List),
                BankId = 0,
                Cr_Amount = 0,
                AddDate = DateTime.Today,
                TransactionDate = DateTime.Today,
                IsDeleted = false,
                PartnerId = 0,
                InvoiceId = 0,
                AccountName = "",

            };
            VoucherDetail _DiscountTakenCreditEntry = new VoucherDetail()
            {
                RefCompanyId = 0,
                VoucherId = 0,
                RefChartOfAccountId = await GetLinkedAccountId((int)LinkedAccountEnum.Discount_Taken, companyId),
                SrNo = 2,
                Note = "",
                Dr_Amount = 0,
                BankId = 0,
                Cr_Amount = DiscountTaken,// _InvoiceDetailListObj.Sum(m => m.Rate).Value - _PaymentVoucherMaster.Voucher_Detail.Sum(m => m.Dr_Amount).Value,
                AddDate = DateTime.Today,
                TransactionDate = DateTime.Today,
                IsDeleted = false,
                PartnerId = 0,
                InvoiceId = 0,
                AccountName = "",

            };
            _PaymentVoucherMaster.VoucherDetails.Add(_DiscountTakenCreditEntry);
            _PaymentVoucherMaster.VoucherDetails.Add(_PaymentVoucherDetailCreditEntry);
            _PaymentVoucherMaster.VoucherDetails.Add(_PaymentVoucherDetailSaleTaxDebitEntry);

            Voucher.Add(_PurchaseVoucherMaster);
            Voucher.Add(_PaymentVoucherMaster);

            if (invoice.Id > 0)
            {//await UpdateInvoiceDetails(invoice);
                await _invoiceService.UpdateInvoiceDetails(invoice);
            }
            else
            {
                long invoiceId = await _invoiceService.CreatePRInvoice(invoice, Voucher);
                //insert invoice Id into voucher and voucher detail
                foreach(var voucher in Voucher)
                {
                    voucher.InvoiceId = invoiceId;
                    if(voucher.VoucherDetails.Count()>0)
                    {
                        foreach(var vouncherDetail in voucher.VoucherDetails)
                        {
                            vouncherDetail.InvoiceId = invoiceId;
                        }
                    }
                }
                foreach(var voucher in Voucher)
                {
                    var voucherId= await _voucherRepository.InsertAndGetIdAsync(voucher);
                //    foreach (var vouncherDetail in voucher.VoucherDetails)
                //    {

                //        //vouncherDetail.InvoiceId = invoiceId;
                //        await _voucherDetailRepository.InsertAndGetIdAsync(vouncherDetail);
                //    }
                }
               

            }
        }

        public async Task UpdateInvoiceDetails(Invoice invoice)
        {
            invoice.IsPaid = false;
            invoice.InvoiceStatus = (InvoiceStatus?)1;
            await _invoiceRepository.UpdateAsync(invoice);
            foreach (var item in invoice.InvoiceDetails)
            {
                item.IsPaid = false;
                await _invoiceDetailRepository.UpdateAsync(item);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private async Task<long> GetCompanyId( long tenatId)
        {
            var comanyId = await _companyRepository.GetAll().Where(a => a.TenantId == tenatId).Select(a => a.Id).FirstOrDefaultAsync();
            return comanyId;
        }


        public async Task<long> GetCOA_Id(string accountDescription, long companyId)
        {
            var linkedAccount = await  _chartOfAccountRepository.GetAll().Where(a => a.AccountDescription == accountDescription  && a.CompanyId== companyId).Select(a => a.Id).FirstOrDefaultAsync();
            return linkedAccount;
        }
        private async Task<long> GetLinkedAccountId(int id, long companyId)
        {
            var linkedAccount = await _linkedAccountRepository.GetAll().Where(a => a.Id == id && a.CompanyID == companyId).Select(a => a.Id).FirstOrDefaultAsync();
            return linkedAccount;
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
        
    }
}
