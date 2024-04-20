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

namespace AccountingBlueBook.AppServices.PurchaseInvoice
{
    public class PurchaseInvoiceAppService : AccountingBlueBookAppServiceBase
    {
        private readonly IRepository<Voucher, long> _voucherRepository;
        private readonly IRepository<VoucherDetail, long> _voucherDetailRepository;
        private readonly IRepository<Entities.MainEntities.Invoices.Invoice, long> _invoiceRepository;
        private readonly InvoiceAppService _invoiceService;
        private readonly IRepository<InvoiceDetail, long> _invoiceDetailRepository;
        public PurchaseInvoiceAppService(IRepository<Entities.MainEntities.Invoices.Invoice, long> invoiceRepository, InvoiceAppService invoiceService, IRepository<InvoiceDetail,
            long> invoiceDetailRepository, IRepository<Voucher, long> voucherRepository, IRepository<VoucherDetail, long> voucherDetailRepository)
        {
            _voucherRepository = voucherRepository;
            _voucherDetailRepository = voucherDetailRepository;
            _invoiceRepository = invoiceRepository;
            _invoiceDetailRepository = invoiceDetailRepository;
            _invoiceService = invoiceService;
        }
        public async Task SavePurchaseInvoice(SavePurchaseInvoice input)
        {
            try
            {
                List<Voucher> _VouchersList = new List<Voucher>();
                List<InvoiceDetail> _InvoiceList = new List<InvoiceDetail>();
                var invoice = new Entities.MainEntities.Invoices.Invoice
                {
                    TenantId = (int)AbpSession.TenantId,
                    RefCompanyId = (int)AbpSession.TenantId,
                    RefSupplierId = input.VendorId,
                    RefrenceNo = input.RefNo,
                    RefTermId = input.RefTermID,
                    InvoiceDate = input.PurchaseInvoiceDate,
                    InvoiceDueDate = input.InvoiceDueDate,
                    InvoiceType = InvoiceType.Purchase_Invoice,
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
                        //RefChartOfAccountId = item.RefChartOfAccountID,
                        RefCustomerId = item.RefCustomerID,
                        //InvoiceId = (long)item.RefInvoiceID,
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
                        //InvoiceId = (long)item.RefInvoiceID,
                        RefPaidInvoiceId = item.RefPaidInvoiceID,
                        RefProducId = 0,
                        SaleTax = 0
                    };
                    _InvoiceList.Add(_IDL);                    
                }
                //////////////////////////Start-Sale-Voucher///////////////////////////////
                Voucher _PurchaseVoucherMaster = new Voucher()
                {
                    VoucherNo = "",
                    VoucherTypeCode = "PV",
                    AddDate = DateTime.Today,
                    TransactionDate = DateTime.Today,
                    LastModificationTime = DateTime.Today,
                    CreatorUserId = 0,
                    LastModifierUserId = 0,
                    PaymentMode = 0,
                    IsDeleted = false,
                    PaymentType = 0,

                };
                VoucherDetail _SaleVoucherDetailSaletaxDebitEntry = new VoucherDetail()
                {
                    RefCompanyId = 0,
                    VoucherId = 0,
                    //RefChartOfAccountId = new BL_Invoice().Get_Linked_AccountID((int)eLinkedAccounts.Sale_Tax_Payables),
                    SrNo = 1,
                    Note = "",
                    Dr_Amount = CalculateSaletax(_InvoiceList),
                    BankId = 0,
                    Cr_Amount = 0,
                    AddDate = DateTime.Today,
                    TransactionDate = DateTime.Today,
                    LastModificationTime = DateTime.Today,
                    CreatorUserId = 1,
                    LastModifierUserId = 1,
                    IsDeleted = false,
                    PartnerId = 0,
                    InvoiceId = 0,
                    AccountName = "",
                };

                foreach (var item in _InvoiceList)
                {
                    if (item.RefProducId != 0 && item.RefProducId != null)
                    {
                        long? productid = item.RefProducId;
                       //long? ExpenseAccountId = new BL_Products().Get_Active_Products_List().Where(x => x.ProductID == productid).FirstOrDefault().ExpenseAccId;
                        VoucherDetail _PurchaseVoucherDetailDebitEntry = new VoucherDetail()
                        {
                            RefCompanyId = 0,
                            VoucherId = 0,
                           // ref_ChartOfAccountID = ExpenseAccountId == 0 ? new BL_Invoice().Get_Linked_AccountID((int)eLinkedAccounts.Purchases) : ExpenseAccountId,
                            SrNo = 2,
                            Note = "",
                            Dr_Amount = ((item.Rate.HasValue ? item.Rate.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0)) - (((item.Rate.HasValue ? item.Rate.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0) * (item.Discount.HasValue ? item.Discount.Value : 0)) / 100),
                            BankId = 0,
                            Cr_Amount = 0,
                            AddDate = DateTime.Today,
                            TransactionDate = DateTime.Today,
                            LastModificationTime = DateTime.Today,
                            CreatorUserId = 1,
                            LastModifierUserId = 1,
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
                          // ref_ChartOfAccountID = item.ref_ChartOfAccountID,
                            SrNo = 2,
                            Note = "",
                            Dr_Amount = item.Amount,
                            BankId = 0,
                            Cr_Amount = 0,
                            AddDate = DateTime.Today,
                            TransactionDate = DateTime.Today,
                            LastModificationTime = DateTime.Today,
                            CreatorUserId = 1,
                            LastModifierUserId = 1,
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
                    //ref_ChartOfAccountID = new BL_Invoice().Get_Linked_AccountID((int)eLinkedAccounts.Vendor_Payables),
                    SrNo = 3,
                    Note = "",
                    Dr_Amount = 0,
                    BankId = 0,
                    //  Cr_Amount = _InvoiceObj.Total,
                    AddDate = DateTime.Today,
                    TransactionDate = DateTime.Today,
                    LastModificationTime = DateTime.Today,
                    CreatorUserId = 1,
                    LastModifierUserId = 1,
                    IsDeleted = false,
                    // PartnerId = _InvoiceObj.ref_SupplierID,
                    InvoiceId = 0,
                    AccountName = "",

                };
                _PurchaseVoucherMaster.VoucherDetails.Add(_SaleVoucherDetailSaletaxDebitEntry);
                _PurchaseVoucherMaster.VoucherDetails.Add(_PurchaseVoucherDetailCreditEntry);
                //////////////////////////End-Sale-Voucher///////////////////////////////

                long invoiceId = await _invoiceService.CreateInvoice(invoice);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred while creating Purchase Invoice", ex.Message);
            };
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
