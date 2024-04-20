using Abp.Domain.Repositories;
using AccountingBlueBook.AppServices.Invoices;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using AccountingBlueBook.Entities.MainEntities.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.UI;
using AccountingBlueBook.AppServices.PurchaseInvoice.Dto;
using AccountingBlueBook.Enums;
using Microsoft.EntityFrameworkCore;
using AccountingBlueBook.AppServices.ReceivedPayment.Dto;
using AccountingBlueBook.AppServices.ReceivedPayment;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.AppServices.PurchaseReceipt;

namespace AccountingBlueBook.AppServices.PurchaseInvoice
{
    public class PurchaseInvoiceService : AccountingBlueBookAppServiceBase, IPurchaseInvoiceService
    {
        private readonly IRepository<Voucher, long> _voucherRepository;
        private readonly IRepository<VoucherDetail, long> _voucherDetailRepository;
        private readonly IRepository<Entities.MainEntities.Invoices.Invoice, long> _invoiceRepository;
        private readonly InvoiceAppService _invoiceService;
        private readonly PurchaseReceiptService _purchaseReceiptService;
        private readonly ReceivedPaymentService _receivedPayment;
        private int tenantId = 0;
        private readonly IRepository<InvoiceDetail, long> _invoiceDetailRepository;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<Entities.MainEntities.ProductService> _productServiceRepository;
        public PurchaseInvoiceService(IRepository<Entities.MainEntities.Invoices.Invoice, long> invoiceRepository, InvoiceAppService invoiceService, IRepository<InvoiceDetail,
            long> invoiceDetailRepository, IRepository<Voucher, long> voucherRepository, IRepository<VoucherDetail, long> voucherDetailRepository, ReceivedPaymentService receivedPayment,
            IRepository<Transaction> transactionRepository, IRepository<Vendor> vendorRepository, PurchaseReceiptService purchaseReceiptService,
            IRepository<Entities.MainEntities.ProductService> productServiceRepository)
        {
            _voucherRepository = voucherRepository;
            _voucherDetailRepository = voucherDetailRepository;
            _invoiceRepository = invoiceRepository;
            _invoiceDetailRepository = invoiceDetailRepository;
            _invoiceService = invoiceService;
            _receivedPayment = receivedPayment;
            _transactionRepository = transactionRepository;
            _vendorRepository = vendorRepository;
            _purchaseReceiptService = purchaseReceiptService;
            _productServiceRepository = productServiceRepository;
        }
        public async Task<long> SavePurchaseInvoice(SavePurchaseInvoice input)
        {
            try
            {
                long InvoiceId = input.InvoiceId ?? 0;
                List<Voucher> _VouchersList = new List<Voucher>();
                List<InvoiceDetail> _InvoiceList = new List<InvoiceDetail>();
                var invoice = new Entities.MainEntities.Invoices.Invoice
                {
                    Id = input.InvoiceId == null ? 0 : (long)input.InvoiceId,
                    TenantId = (int)AbpSession.TenantId,
                    RefCompanyId = (int)AbpSession.TenantId,
                    RefSupplierId = input.VendorId,
                    RefTermId = input.RefTermID,
                    RefrenceNo = input.RefNo,
                    InvoiceDate = input.PurchaseInvoiceDate,
                    InvoiceDueDate = input.InvoiceDueDate,
                    InvoiceType = InvoiceType.Purchase_Invoice,
                    InvoiceNo = input.InvoiceNo,
                    Total = input.Total,
                    Note = input.Note,
                    IsPaid = false,
                    IsActive = true
                };

                foreach (var item in input.PurchaseInvoice)
                {
                    InvoiceDetail _IDL = new InvoiceDetail()
                    {
                        Id = item.InvoiceDetailID,
                        Amount = (long?)item.Amount,
                        Description = item.Description,
                        IsPaid = item.IsPaid,
                        Discount = (long?)item.Discount,
                        PaidAmount = item.PaidAmount,
                        Quantity = item.Quantity,
                        Rate = (long?)item.Rate,
                        RefChartOfAccountId = item.RefChartOfAccountID,
                        RefCustomerId = item.RefCustomerID,
                        RefPaidInvoiceId = item.RefPaidInvoiceID,
                        RefProducId = item.RefProducID,
                        SaleTax = (long?)item.SaleTax
                    };
                    _InvoiceList.Add(_IDL);
                }
                foreach (var item in input.PurchaseInvoiceAccount)
                {
                    InvoiceDetail _IDL = new InvoiceDetail()
                    {
                        Id = item.InvoiceDetailID,
                        Amount = (long?)item.Amount,
                        Description = item.Description,
                        IsPaid = item.IsPaid,
                        Discount = 0,
                        PaidAmount = item.PaidAmount,
                        Quantity = 0,
                        Rate = 0,
                        RefChartOfAccountId = item.RefChartOfAccountID,
                        RefCustomerId = item.RefCustomerID,
                        RefPaidInvoiceId = item.RefPaidInvoiceID,
                        RefProducId = 0,
                        SaleTax = 0
                    };
                    _InvoiceList.Add(_IDL);
                }
                invoice.InvoiceDetails = _InvoiceList;
                //////////////////////////Start-Sale-Voucher///////////////////////////////
               
                var PurchasesLinkedAccountId = 0; // Purchases
                var VendorPayablesLinkedAccountId = 0; //Vendor Payable //await _purchaseReceiptService.GetCOA_Id("Vendor Payable", invoice.RefCompanyId);
                Voucher _PurchaseVoucherMaster = new Voucher()
                {
                    VoucherNo = "",
                    VoucherTypeCode = "PV",
                    AddDate = DateTime.Today,
                    TransactionDate = DateTime.Today,
                    LastModificationTime = DateTime.Today,
                    CreatorUserId = AbpSession.UserId,
                    LastModifierUserId = 0,
                    PaymentMode = 0,
                    IsDeleted = false,
                    PaymentType = 0,

                };
                VoucherDetail _SaleVoucherDetailSaletaxDebitEntry = new VoucherDetail()
                {
                    RefCompanyId = 0,
                    VoucherId = 0,
                    RefChartOfAccountId = 0, // Sale_Tax_Payables,
                    SrNo = 1,
                    Note = "",
                    Dr_Amount = CalculateSaletax(_InvoiceList),
                    BankId = 0,
                    Cr_Amount = 0,
                    AddDate = DateTime.Today,
                    TransactionDate = DateTime.Today,
                    LastModificationTime = DateTime.Today,
                    CreatorUserId = AbpSession.UserId,
                    LastModifierUserId = 0,
                    IsDeleted = false,
                    PartnerId = 0,
                    InvoiceId = 0,
                    AccountName = "",
                };

                foreach (var item in _InvoiceList)
                {
                    if (item.RefProducId != 0 && item.RefProducId != null)
                    {
                        var productid = item.RefProducId;
                        var ExpenseAccountId = _productServiceRepository.FirstOrDefault((int)productid).ExpenseAccountId;
                        VoucherDetail _PurchaseVoucherDetailDebitEntry = new VoucherDetail()
                        {
                            RefCompanyId = 0,
                            VoucherId = 0,
                            RefChartOfAccountId = ExpenseAccountId == 0 ? PurchasesLinkedAccountId : ExpenseAccountId,
                            SrNo = 2,
                            Note = "",
                            Dr_Amount = ((item.Rate.HasValue ? item.Rate.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0)) - (((item.Rate.HasValue ? item.Rate.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0) * (item.Discount.HasValue ? item.Discount.Value : 0)) / 100),
                            BankId = 0,
                            Cr_Amount = 0,
                            AddDate = DateTime.Today,
                            TransactionDate = DateTime.Today,
                            LastModificationTime = DateTime.Today,
                            CreatorUserId = AbpSession.UserId,
                            LastModifierUserId = 0,
                            IsDeleted = false,
                            PartnerId = 0,
                            InvoiceId = 0,
                            AccountName = "",

                        };
                        _PurchaseVoucherMaster.VoucherDetails.Add(_PurchaseVoucherDetailDebitEntry);
                    }
                    else
                    {
                        VoucherDetail _PurchaseVoucherDetailDebitEntry = new VoucherDetail()
                        {

                            RefCompanyId = 0,
                            VoucherId = 0,
                            RefChartOfAccountId = item.RefChartOfAccountId,
                            SrNo = 2,
                            Note = "",
                            Dr_Amount = item.Amount,
                            BankId = 0,
                            Cr_Amount = 0,
                            AddDate = DateTime.Today,
                            TransactionDate = DateTime.Today,
                            LastModificationTime = DateTime.Today,
                            CreatorUserId = AbpSession.UserId,
                            LastModifierUserId = 0,
                            IsDeleted = false,
                            PartnerId = 0,
                            InvoiceId = 0,
                            AccountName = "",

                        };
                        _PurchaseVoucherMaster.VoucherDetails.Add(_PurchaseVoucherDetailDebitEntry);
                    }
                }

                VoucherDetail _PurchaseVoucherDetailCreditEntry = new VoucherDetail()
                {

                    RefCompanyId = 0,
                    VoucherId = 0,
                    RefChartOfAccountId = VendorPayablesLinkedAccountId,
                    SrNo = 3,
                    Note = "",
                    Dr_Amount = 0,
                    BankId = 0,
                    Cr_Amount = input.Total,
                    AddDate = DateTime.Today,
                    TransactionDate = DateTime.Today,
                    LastModificationTime = DateTime.Today,
                    CreatorUserId = AbpSession.UserId,
                    LastModifierUserId = 0,
                    IsDeleted = false,
                    PartnerId = invoice.RefSupplierId,
                    InvoiceId = 0,
                    AccountName = "",

                };
                _PurchaseVoucherMaster.VoucherDetails.Add(_SaleVoucherDetailSaletaxDebitEntry);
                _PurchaseVoucherMaster.VoucherDetails.Add(_PurchaseVoucherDetailCreditEntry);
                //////////////////////////End-Sale-Voucher///////////////////////////////
                if (invoice.Id > 0)
                {
                    await _invoiceService.UpdateInvoiceDetails(invoice);
                }
                else
                {
                    InvoiceId = await _invoiceService.CreateInvoice(invoice);
                    long voicherId = await _invoiceService.AddNewVouchers(InvoiceId, _PurchaseVoucherMaster);
                }
                return InvoiceId;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred while creating Purchase Invoice", ex.Message);
            };
        }
        public async Task UpdatepurchaseInvoice(Entities.MainEntities.Invoices.Invoice invoice)
        {
            await _invoiceRepository.UpdateAsync(invoice);
            foreach (var item in invoice.InvoiceDetails)
            {
                if (item.Id > 0)
                {
                    await _invoiceDetailRepository.UpdateAsync(item);
                }
                else
                {
                    await _invoiceDetailRepository.InsertAndGetIdAsync(item);
                }
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<long> SavePurchasePayment(SaveReceivedPayment input, int tenantId = 0)
        {
            try
            {
                this.tenantId = tenantId;
                var vendor = await _vendorRepository.FirstOrDefaultAsync(x => x.Id == input.RefSupplierID);
                var InvoiceId = input.InvoiceID;
                ChargeCardDto card_obj = new ChargeCardDto();
                if (input.ChargeCard != null)
                {
                    if (!String.IsNullOrWhiteSpace(input.ChargeCard.CardNumber))
                    {
                        input.ChargeCard.Amount = input.Total;
                        bool IsAmountCharged = await _receivedPayment.SaveChargeCard(input.ChargeCard);
                        if (IsAmountCharged == true)
                        {
                            InvoiceId = await InvoiceDetails(input, vendor);
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        InvoiceId = await InvoiceDetails(input, vendor);
                    }
                }
                else
                {
                    InvoiceId = await InvoiceDetails(input, vendor);
                }
                return InvoiceId ?? 0;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred while creating purchase payment", ex.Message);
            };
        }
        public async Task<long> InvoiceDetails(SaveReceivedPayment input, Vendor vendor)
        {
            var invoice = new Invoice
            {
                Id = input.Id == null ? 0 : (long)input.Id,
                TenantId = AbpSession.TenantId != null ? (int)AbpSession.TenantId : tenantId,
                Email = input.Emails.Count() > 0 ? string.Join(",", input.Emails) : null,
                RefCompanyId = AbpSession.TenantId != null ? (int)AbpSession.TenantId : tenantId,
                RefCustomerId = vendor.Id,
                RefPaymentMethodId = input.RefPaymentMethodID,
                RefDepositToAccountId = input.RefDepositToAccountId,
                PaymentDate = input.PaymentDate,
                RefrenceNo = input.ReferenceNo,
                InvoiceDate = System.DateTime.Now,
                Total = input.Total,
                PaidAmount = input.PaidAmount,
                InvoiceType = InvoiceType.Purchase_Payment,
                Note = input.Note,
                InvoiceGroupId = (int?)input.InvoiceID,
                InvoiceNo = input.InvoiceNo,
                IsSendLater = input.IsSendLater,
                InvoiceStatus = (InvoiceStatus?)input.RefInvoiceStatus,
                IsPaid = true,
                IsActive = true
            };
            List<InvoiceDetail> List = input.ReceivedPayments.Select(data => new InvoiceDetail
            {
                Id = data.InvoiceDetailId == null ? 0 : (long)data.InvoiceDetailId,
                TenantId = AbpSession.TenantId != null ? (int)AbpSession.TenantId : tenantId,
                RefPaidInvoiceId = data.InvoiceID,
                RefProducId = data.RefProducID,
                PaidAmount = data.PaidAmount,
                RefChartOfAccountId = input.RefDepositToAccountId,
                RefCustomerId = vendor.Id,
                IsPaid = true
            }).ToList();

            invoice.InvoiceDetails = List;

            var voucher = new Voucher()
            {
                VoucherNo = "",
                VoucherTypeCode = "EV",
                AddDate = System.DateTime.Today,
                TransactionDate = DateTime.Today,
                LastModificationTime = DateTime.Today,
                CreatorUserId = AbpSession.UserId,
                LastModifierUserId = 0,
                PaymentMode = 0,
                IsDeleted = false,
                PaymentType = 0,
                TenantId = AbpSession.TenantId != null ? (int)AbpSession.TenantId : tenantId
            };
            voucher.VoucherDetails.Add(new VoucherDetail()
            {
                RefCompanyId = 0,
                VoucherId = 0,
                RefChartOfAccountId = 82, //Customers_Receivables,
                SrNo = 1,
                Note = "",
                Dr_Amount = 0,
                BankId = 0,
                Cr_Amount = invoice.Total,
                AddDate = DateTime.Today,
                TransactionDate = DateTime.Today,
                LastModificationTime = DateTime.Today,
                CreatorUserId = AbpSession.UserId,
                LastModifierUserId = 0,
                IsDeleted = false,
                PartnerId = invoice.RefCustomerId,
                InvoiceId = 0,
                AccountName = "",
                TenantId = AbpSession.TenantId != null ? (int)AbpSession.TenantId : tenantId
            });
            voucher.VoucherDetails.Add(new VoucherDetail()
            {
                RefCompanyId = 0,
                VoucherId = 0,
                RefChartOfAccountId = input.RefDepositToAccountId,
                SrNo = 2,
                Note = "",
                Dr_Amount = invoice.Total,
                BankId = 0,
                Cr_Amount = 0,
                AddDate = System.DateTime.Today,
                TransactionDate = DateTime.Today,
                LastModificationTime = DateTime.Today,
                CreatorUserId = AbpSession.UserId,
                LastModifierUserId = 0,
                IsDeleted = false,
                PartnerId = invoice.RefCustomerId,
                InvoiceId = 0,
                AccountName = "",
                TenantId = AbpSession.TenantId != null ? (int)AbpSession.TenantId : tenantId
            });
            voucher.VoucherDetails.Add(new VoucherDetail()
            {
                RefCompanyId = 0,
                VoucherId = 0,
                RefChartOfAccountId = 0, //Discount_Taken,
                SrNo = 2,
                Note = "",
                Dr_Amount = 0,
                BankId = 0,
                Cr_Amount = List.Sum(m => m.Amount).Value - voucher.VoucherDetails.Sum(m => m.Cr_Amount).Value,
                AddDate = System.DateTime.Today,
                TransactionDate = DateTime.Today,
                LastModificationTime = DateTime.Today,
                CreatorUserId = AbpSession.UserId,
                LastModifierUserId = 0,
                IsDeleted = false,
                PartnerId = invoice.RefCustomerId,
                InvoiceId = 0,
                AccountName = "",
                TenantId = AbpSession.TenantId != null ? (int)AbpSession.TenantId : tenantId
            });

            if (invoice.Id > 0)
            {
                 await UpdateInvoiceDetails(invoice,input);
            }
            else
            {
                long invoiceId = await _invoiceService.CreateInvoice(invoice);
                long voicherId = await _invoiceService.AddNewVouchers(invoiceId, voucher);

                var invoiceData = await _invoiceRepository.FirstOrDefaultAsync(x => x.Id == invoiceId);
                if (invoiceData.RefrenceNo != null && invoiceData.RefrenceNo != "")
                {
                    var transaction = new Transaction
                    {
                        RefCustomerID = invoiceData.RefCustomerId,
                        ReferalNo = invoiceData.RefrenceNo,
                        TransactionDate = System.DateTime.Now,
                        RefCompanyID = invoiceData.RefCompanyId,
                        TranDescription = invoiceData.Note,
                        CreditAmount = invoiceData.Total,
                        ImportFlag = false,
                        Status = "Unverified",
                        IsDeleted = false,
                        BankId = (int?)invoiceData.RefDepositToAccountId,
                        PaymentReceiveID = (int?)invoiceData.Id,
                        PaymentReceiveNo = invoiceData.InvoiceNo,
                        InvoiceTypeID = (int?)invoiceData.InvoiceType,
                    };

                    await _transactionRepository.InsertOrUpdateAndGetIdAsync(transaction);
                }
                return invoiceId;
            }
            return invoice.Id;
        }
        public async Task UpdateInvoiceDetails(Invoice invoice, SaveReceivedPayment input)
        {
            var _invoice = await _invoiceRepository.FirstOrDefaultAsync(x => x.Id == invoice.Id);
            _invoice.PaymentDate = invoice.PaymentDate;
            _invoice.RefPaymentMethodId = invoice.RefPaymentMethodId;
            _invoice.RefrenceNo = invoice.RefrenceNo;
            _invoice.RefDepositToAccountId = invoice.RefDepositToAccountId;
            _invoice.Note = invoice.Note;
            _invoice.IsSendLater = invoice.IsSendLater;
            _invoice.Email = invoice.Email;
            await _invoiceRepository.UpdateAsync(_invoice);

            var _invoiceDetail = await _invoiceDetailRepository.GetAll().Where(x => x.InvoiceId == invoice.Id).ToListAsync();
            foreach (var item in _invoiceDetail)
            {
                if (!input.ReceivedPayments.Any(sp => sp.InvoiceDetailId == item.Id))
                {
                    item.IsPaid = false;
                }
            }

            foreach (var item in input.ReceivedPayments)
            {
                var invoiceList = await _invoiceRepository.FirstOrDefaultAsync(x => x.InvoiceNo == item.InvoiceNo);
                invoiceList.InvoiceNo = item.InvoiceNo;
                await _invoiceRepository.UpdateAsync(invoiceList);
                var invoiceDetail = await _invoiceDetailRepository.FirstOrDefaultAsync(x => x.Id == item.InvoiceDetailId);
                invoiceDetail.PaidAmount = item.PaidAmount;
                await _invoiceDetailRepository.UpdateAsync(invoiceDetail);
            };

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public decimal CalculateSaletax(List<InvoiceDetail> objList)
        {
            try
            {
                decimal TotalSaleTax = 0M;
                for (int i = 0; i < objList.Count(); i++)
                {
                    if (objList[i].RefProducId.HasValue && objList[i].RefProducId != 0)
                    {
                        decimal TotalRate = (objList[i].Rate.HasValue ? objList[i].Rate.Value : 0) * (objList[i].Quantity.HasValue ? objList[i].Quantity.Value : 0);
                        decimal TotalRateAfterDiscount = TotalRate - (TotalRate * (objList[i].Discount.HasValue ? objList[i].Discount.Value : 0) / 100);
                        TotalSaleTax += (TotalRateAfterDiscount * (objList[i].SaleTax.HasValue ? objList[i].SaleTax.Value : 0) / 100);
                    }
                }
                return TotalSaleTax;
            }
            catch (Exception ex)
            {
                Logger.Error("CalculateSaletax method failed in InvoiceAppService", ex);
                return -1;
            }
        }
    }
}
