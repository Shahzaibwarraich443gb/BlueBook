using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AccountingBlueBook.AppServices.CreditNote.Dto;
using AccountingBlueBook.AppServices.Invoices;
using AccountingBlueBook.AppServices.PurchaseReceipt.Dto;
using AccountingBlueBook.AppServices.ReceivedPayment;
using AccountingBlueBook.AppServices.ReceivedPayment.Dto;
using AccountingBlueBook.Authorization.Users;
using AccountingBlueBook.Authorization.Users.Dto;
using AccountingBlueBook.CardConnectConfiguration;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using AccountingBlueBook.Entities.MainEntities.LinkedAccounts;
using AccountingBlueBook.Entities.MainEntities.Vouchers;
using AccountingBlueBook.Entities.NormalizeEntities;
using AccountingBlueBook.Enums;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.CreditNote
{
    [AbpAuthorize]
    public class CreditNoteService : AccountingBlueBookAppServiceBase, ICreditNoteService
    {
        private readonly IRepository<ProductService> _productServiceRepository;
        private readonly IRepository<ChartOfAccount> _chartOfAccountRepository;
        private readonly IRepository<Company> _companyRepository;
        private int tenantId = 0;
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
        public CreditNoteService(IRepository<Transaction> transactionRepository, IRepository<CardTransaction> cardTransactionRepository,
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

        //public async Task SavePurchaseReceipt(SaveCreditNoteDto input)
        //{
        //    try
        //    {
        //        var supplier = await _vendorRepository.FirstOrDefaultAsync(x => x.Id == input.RefSupplierId);
        //        ChargeCardDto card_obj = new ChargeCardDto();
        //        if (input.ChargeCard != null)
        //        {
        //            if (!String.IsNullOrWhiteSpace(input.ChargeCard.CardNumber))
        //            {
        //                input.ChargeCard.Amount = input.Total;
        //                //input.ChargeCard.CustomerEmail = customer.Email;
        //                var IsAmountCharged = await _receivePaymentService.SaveChargeCard(input.ChargeCard);
        //                if (IsAmountCharged == true)
        //                {
        //                    await InvoiceDetails(input, supplier);
        //                }
        //                else
        //                {
        //                    return;
        //                }
        //            }
        //            else
        //            {
        //                await InvoiceDetails(input, supplier);
        //            }
        //        }
        //        else
        //        {
        //            await InvoiceDetails(input, supplier);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new UserFriendlyException("An error occurred while creating payment received", ex.Message);
        //    };
        //}

        public async Task SaveCreditNote(SaveCreditNoteDto input)
        {
            var customer = await _customerRepository.FirstOrDefaultAsync(x => x.Id == input.RefCustomerID);
            var invoice = new Invoice
            {
                Id = input.InvoiceID == null ? 0 : (long)input.InvoiceID,
                RefCustomerId = customer.Id,
                TenantId = AbpSession.TenantId != null ? (int)AbpSession.TenantId : tenantId,
                Email = input.Emails.Count() > 0 ? string.Join(",", input.Emails) : null,
                RefCompanyId = AbpSession.TenantId != null ? (int)AbpSession.TenantId : tenantId,
                CreditNoteDate = input.CreditNoteDate,
                InvoiceStatus = (InvoiceStatus?)input.RefInvoiceStatus,
                IsSendLater = input.IsSendLater,
                Total = input.Total,
                InvoiceType = (InvoiceType)input.RefInvoiceType,
                Note = input.Note,
                InvoiceGroupId = (int?)input.InvoiceID,
                InvoiceNo = input.InvoiceNo,
                IsPaid = false

            };
            List<InvoiceDetail> List = input.creditNoteDto.Select(data => new InvoiceDetail
            {
                Id = data.Id == null ? 0 : (long)data.Id,
                InvoiceId = input.InvoiceID == null ? 0 : (long)input.InvoiceID,
                Amount = data.Amount,
                Discount = data.Discount,
                Quantity = data.Quantity,
                Rate = data.Rate,
                RefProducId = data.RefProducID,
                SaleTax = data.SaleTax,
                Description = data.Description,
                RefCustomerId = input.Id
            }).ToList();

            invoice.InvoiceDetails = List;
            var tenatId = (int)AbpSession.TenantId;
            var companyId = await GetCompanyId(tenatId);






            //var voucher = new Voucher();
            Voucher _SaleVoucherMaster = new Voucher()
            {

                VoucherNo = "",
                VoucherTypeCode = "SR",
                AddDate = DateTime.Today,
                TransactionDate = DateTime.Today,
                PaymentMode = 0,
                CreatorUserId = 0,
                LastModifierUserId = 0,
                IsDeleted = false,
                LastModificationTime = DateTime.Today,
                PaymentType = 0,
            };


           VoucherDetail _SaleVoucherDetailCreditEntry = new VoucherDetail()
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
                CreatorUserId = 0,
                LastModifierUserId = 0,
                IsDeleted = false,
                PartnerId = invoice.RefCustomerId,
                LastModificationTime = DateTime.Today,
                InvoiceId = 0,
                AccountName = "",
            };

            VoucherDetail _SaleVoucherDetailSaletaxDebitEntry = new VoucherDetail()
            {

                RefCompanyId = 0,
                VoucherId = 0,
                //RefChartOfAccountId = new BL_Invoice().Get_Linked_AccountID((int)eLinkedAccounts.Sale_Tax_Payables),
                SrNo = 2,
                Note = "",
                Dr_Amount = CalculateSaletax(List),
                BankId = 0,
                Cr_Amount = 0,
                AddDate = DateTime.Today,
                TransactionDate = DateTime.Today,
                LastModificationTime = DateTime.Today,
                CreatorUserId = 0,
                LastModifierUserId = 0,
                IsDeleted = false,
                PartnerId = 0,
                InvoiceId = 0,
                AccountName = "",

            };
            foreach (var item in List)
            {
                var productIds = input.creditNoteDto.Select(a => a.RefProducID);

                var incomeAccoundIds = await _productServiceRepository.GetAll().Where(a => productIds.Any(x => x == a.Id)).Select(a => a.Id).ToListAsync();
                VoucherDetail _SaleVoucherDetailDebitEntry = new VoucherDetail()
                {
                    RefCompanyId = 0,
                    VoucherId = 0,
                    //ref_ChartOfAccountID = new BL_Invoice().Get_Linked_AccountID((int)eLinkedAccounts.Sale_Return),
                    SrNo = 2,
                    Note = "",
                    Dr_Amount = ((item.Rate.HasValue ? item.Rate.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0)) - (((item.Rate.HasValue ? item.Rate.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0) * (item.Discount.HasValue ? item.Discount.Value : 0)) / 100),
                    BankId = 0,
                    Cr_Amount = 0,
                    AddDate = DateTime.Today,
                    TransactionDate = DateTime.Today,
                    LastModificationTime = DateTime.Today,
                    //ModifiedDate = DateTime.Today,
                    //AddedByUserID = 1,
                    //ModifiedByUserID = 1,
                    CreatorUserId = 0,
                    LastModifierUserId = 0,
                    IsDeleted = false,
                    PartnerId = 0,
                    InvoiceId = 0,
                    AccountName = "",

                };
                _SaleVoucherMaster.VoucherDetails.Add(_SaleVoucherDetailDebitEntry);

            }
            _SaleVoucherMaster.VoucherDetails.Add(_SaleVoucherDetailCreditEntry);
            _SaleVoucherMaster.VoucherDetails.Add(_SaleVoucherDetailSaletaxDebitEntry);

            long invoiceId;
            if (invoice.Id > 0)
            {
                invoiceId = await _invoiceService.UpdateInvoiceDetails(invoice); //await UpdateInvoice(invoice, new List<Voucher>() { voucher });
            }
            else
            {
                invoiceId = await _invoiceService.CreateInvoice(invoice);
                long voicherId = await _invoiceService.AddNewVouchers(invoiceId, _SaleVoucherMaster);

            }

}



            private async Task<long> GetCompanyId( long tenatId)
        {
            var comanyId = await _companyRepository.GetAll().Where(a => a.TenantId == tenatId).Select(a => a.Id).FirstOrDefaultAsync();
            return comanyId;
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
