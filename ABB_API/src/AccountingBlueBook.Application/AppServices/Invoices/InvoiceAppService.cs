using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using AccountingBlueBook.AppServices.Invoices.dto;
using AccountingBlueBook.AppServices.JournalVoucher.Dto;
using AccountingBlueBook.Authorization.Users;
using AccountingBlueBook.Authorization.Users.Dto;
using AccountingBlueBook.Common.CommonLookupDto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AccountingBlueBook.Entities.MainEntities.Customers.Dto;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using AccountingBlueBook.Entities.MainEntities.LinkedAccounts;
using AccountingBlueBook.Entities.MainEntities.Vouchers;
using AccountingBlueBook.Entities.NormalizeEntities;
using AccountingBlueBook.Enums;
using AccountingBlueBook.Net.Emailing;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AccountingBlueBook.Configuration.AppSettingNames;

namespace AccountingBlueBook.AppServices.Invoices
{
    [AbpAuthorize]
    public class InvoiceAppService : AccountingBlueBookAppServiceBase, IInvoiceAppService
    {
        private readonly IRepository<ProductService> _productServiceRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Address> _addressRepository;

        private readonly IRepository<Voucher, long> _voucherRepository;
        private readonly IRepository<LinkedAccount, long> _linkedAccountRepository;
        private readonly IRepository<VoucherDetail, long> _voucherDetailRepository;
        private readonly IRepository<InvoiceDetail, long> _invoiceDetailRepository;
        private readonly IRepository<ChartOfAccount> _chartOfAccountRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IEmailAppServices _emailAppService;
        private readonly IEmailTemplateProvider _emailTemplateProvider;

        public InvoiceAppService(IRepository<ProductService> productServiceRepository,
                                 IRepository<Customer> customerRepository,
                                 IRepository<Voucher, long> voucherRepository,
                                 IRepository<VoucherDetail, long> voucherDetailRepository,
                                 IRepository<LinkedAccount, long> linkedAccountRepository,
                                 IInvoiceRepository invoiceRepository,
                                 IRepository<InvoiceDetail, long> invoiceDetailRepository,
                                 IRepository<ChartOfAccount> chartOfAccountRepository,
                                 IEmailAppServices emailAppService,
                                 IRepository<Address> addressRepository,
                                 IRepository<Company> companyRepository,
                                 IEmailTemplateProvider emailTemplateProvider)
        {
            _productServiceRepository = productServiceRepository;
            _customerRepository = customerRepository;
            _voucherRepository = voucherRepository;
            _linkedAccountRepository = linkedAccountRepository;
            _voucherDetailRepository = voucherDetailRepository;
            _invoiceDetailRepository = invoiceDetailRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _invoiceRepository = invoiceRepository;
            _emailAppService = emailAppService;
            _addressRepository = addressRepository;
            _companyRepository = companyRepository;
            _emailTemplateProvider = emailTemplateProvider;
        }

        int? jobTenantId = null;

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




        public async Task<long> SaveInvoice(CreateInvoiceDto input)
        {
            try
            {

                //ObjectMapper.Map<Invoice>(input.Invoice);

                if (input.TenantId != null && input.TenantId > 0)
                {
                    jobTenantId = input.TenantId;
                }

                Invoice invoice = new Invoice
                {
                    Id = input.Invoice.InvoiceId == null ? 0 : (long)input.Invoice.InvoiceId,
                    InvoiceDate = input.Invoice.InvoiceDate,
                    InvoiceDueDate = input.Invoice.InvoiceDueDate,
                    RefCustomerId = input.Invoice.RefCustomerID,
                    RefTermId = input.Invoice.RefTermID,
                    InvoiceNo = input.Invoice.InvoiceNo,
                    Email = input.Invoice.Email.Count() > 0 ? string.Join(",", input.Invoice.Email) : null,
                    TenantId = AbpSession.TenantId == null ? 1 : (int)AbpSession.TenantId,
                    RefCompanyId = AbpSession.TenantId == null ? 1 : (int)AbpSession.TenantId,
                    InvoiceType = (InvoiceType?)input.Invoice.RefInvoiceType,
                    //InvoiceStatus = (InvoiceStatus?)input.Invoice.RefInvoiceStatus,
                    IsSendLater = input.Invoice.IsSendLater,
                    Total = input.Invoice.Total,
                    Note = input.Invoice.Note,
                    IsPaid = false,
                    IsDeleted = false
                    //to do: need to change it because tenant id is null when logged in by admin
                };

                List<InvoiceDetail> invoiceDetailList = input.Invoice.InvoiceDetails.Select(data => new InvoiceDetail
                {
                    Id = data.InvoiceDetailId == null ? 0 : (long)data.InvoiceDetailId,
                    InvoiceId = input.Invoice.InvoiceId == null ? 0 : (long)input.Invoice.InvoiceId,
                    Amount = data.Amount,
                    Discount = data.Discount,
                    Quantity = data.Quantity,
                    Rate = data.Rate,
                    RefProducId = data.RefProducID,
                    SaleTax = data.SaleTax,
                    TenantId = 1,
                    RefCustomerId = input.Invoice.RefCustomerID,
                    PaidAmount = data.PaidAmount,
                    Description = data.Description,
                    IsPaid = data.IsPaid
                }).ToList();

                //long amount = 0;

                //foreach(var data in invoiceDetailList)
                //{
                //    amount += (data.Amount ?? 0);
                //    var products = await _productServiceRepository.FirstOrDefaultAsync( x => x.Id == data.RefProducId);
                //    var coaId = products.IncomeAccountId != null ? products.IncomeAccountId : products.ExpenseAccountId;
                //    var coa = await _chartOfAccountRepository.FirstOrDefaultAsync(coaId ?? 0);
                //    coa.b

                //}
                invoice.InvoiceDetails = invoiceDetailList;


                var voucher = new Voucher();
                decimal discountGiven = 0;


                var salesVoucherMaster = new Voucher();
                var productIds = input.Invoice.InvoiceDetails.Select(a => a.RefProducID);

                var incomeAccoundIds = await _productServiceRepository.GetAll().Where(a => productIds.Any(x => x == a.Id)).Select(a => a.IncomeAccountId).ToListAsync();

                // company id is replaced with tenant id
                var companyId = (int)AbpSession.TenantId;


                var linkedAccountId = await GetLinkedAccountId((int)LinkedAccountEnum.Sales_Revenue, companyId);


                #region vouchers

                foreach (var item in input.Invoice.InvoiceDetails)
                {
                    var incomeAccountId = _productServiceRepository.FirstOrDefault((int)item.RefProducID).IncomeAccountId;
                    var expenseAccountId = _productServiceRepository.FirstOrDefault((int)item.RefProducID).ExpenseAccountId;
                    var voucherDetail = new VoucherDetail()
                    {
                        RefCompanyId = 0,
                        VoucherId = 0,
                        RefChartOfAccountId = incomeAccountId != null ? incomeAccountId : expenseAccountId, //incomeAccoundIds.Count(a => a == item.RefProducID) > 0 ? item.RefProducID : linkedAccountId,
                        SrNo = 1,
                        Note = "",
                        Dr_Amount = 0,
                        BankId = 0,
                        Cr_Amount = ((item.Rate.HasValue ? item.Rate.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0)) - (((item.Rate.HasValue ? item.Rate.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0) * (item.Discount.HasValue ? item.Discount.Value : 0)) / 100),
                        AddDate = DateTime.Today,
                        TransactionDate = DateTime.Today,
                        PartnerId = 0,
                        InvoiceId = 0,
                        AccountName = "",
                    };

                    decimal TotalAmount = (item.Rate.HasValue ? item.Rate.Value : 0) * (item.Quantity.HasValue ? item.Quantity.Value : 0);
                    discountGiven = discountGiven + ((TotalAmount * (item.Discount.HasValue ? item.Discount.Value : 0)) / 100);
                    salesVoucherMaster.VoucherDetails.Add(voucherDetail);

                }

                salesVoucherMaster = await AddSpecialVoucherDetails(invoice, voucher, companyId, discountGiven);
                await _voucherRepository.InsertOrUpdateAndGetIdAsync(salesVoucherMaster);

                #endregion

                long invoiceId = 0;
                if (invoice.Id > 0)
                {
                    invoiceId = await UpdateInvoiceDetails(invoice); //await UpdateInvoice(invoice, new List<Voucher>() { voucher });
                }
                else
                {
                    invoiceId = await CreateInvoices(invoice, voucher);
                }

                return invoiceId;
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        public async Task<long> CreateInvoices(Invoice invoice, Voucher voucher)
        {

            try
            {
                invoice.IsActive = true;
                //invoice.IsPaid = true;        
                //EntityDto Id = new EntityDto { Id = (int)invoiceId };

                var invoiceId = await CreateInvoice(invoice);
                var res = await GetPrintDetails(invoiceId);

                if (invoice.IsSendLater == true)
                {
                    if (invoice.Email != "")
                    {
                        string[] totalEmails = invoice.Email.Split(',');
                        for (int i = 0; i < totalEmails.Length; i++)
                        {

                            SenNewEmailIN(totalEmails[i], res);
                        }
                    }
                }
                return invoiceId;

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("An error occurred while creating invoice", ex.Message);
            }
        }
        public async Task SenNewEmailIN(string toEmail, PrintDto transactionItems)
        {
            var TenantId = AbpSession.TenantId;
            string subject = "Thank you for your payment";
            StringBuilder emailbody = new StringBuilder();
            emailbody.Append("<p>Dear Customer,</p>");
            emailbody.Append("<p>Your Invoice has been Added Successfully</p>");
            emailbody.AppendLine();
            emailbody.Append("<p>Regards,</p>");
            emailbody.Append(transactionItems.CompanyName);

            var attachmentStreams = new List<Tuple<string, MemoryStream>>();
            var pdfContent = GeneratePdf(transactionItems);
            attachmentStreams.Add(new Tuple<string, MemoryStream>("Invoice.pdf", pdfContent));
            await _emailAppService.SendMail(new EmailsDto()
            {
                Subject = subject,
                Body = emailbody.ToString(),
                ToEmail = toEmail,
                Streams = attachmentStreams
            });
        }
        private static MemoryStream GeneratePdf(PrintDto transactionItems)
        {
           // var htmlStringNew = "<div #printArea class=\"print-area\" style=\"margin: 25px;\">\r\n    <br />\r\n    <div class=\"row\">\r\n        <div class=\"col-sm-12\">\r\n            <p class=\"uppercase\" style=\"font-weight:600\">" + transactionItems.CompanyName + "</p>\r\n            <p class=\"uppercase\">\r\n              "+ transactionItems.ComAddress +"\r\n            </p>\r\n            <p class=\"uppercase\"> " +transactionItems.ComCity +", "+transactionItems.ComState + " " +transactionItems.ComPostCode + " </p>\r\n            <p class=\"uppercase\">"+ transactionItems.ComCountry+ "</p>\r\n            <p>\r\n                <span style=\"font-weight:600 !important;\">Phone:</span> "+ transactionItems .ComPhone+ "\r\n            </p>\r\n            <p>\r\n            </p>\r\n            <div><span style=\"font-weight:600 !important;\">Email:</span> "+ transactionItems .ComEmail+ "</div>\r\n            <p></p>\r\n        </div>\r\n    </div>\r\n    <br />\r\n    <div class=\"row\">\r\n        <div class=\"col-sm-12\">\r\n            <p class=\"uppercase\" style=\"color:#53abc6;text-align:left; font-size:24px\">Invoice</p>\r\n        </div>\r\n    </div>\r\n    <br />\r\n    <div class=\"row\">\r\n        <div class=\"col-sm-7\">\r\n            <h2 style=\"font-weight:600 !important;\">Recipient</h2>\r\n <p>\r\n"+ transactionItems.CustomerName+ "\r\n  </p>\r\n <p style=\"width:500px;\">\r\n     "+ transactionItems .CustomerAddress+ "\r\n            </p>\r\n            <p class=\"uppercase\"> "+ transactionItems .CustomerCity+ " ,"+ transactionItems.CustomerState+""+ transactionItems.CustomerPostCode + "</p>\r\n            <p class=\"uppercase\">"+ transactionItems .CustomerCountry+ "</p>\r\n            <p>\r\n                <span style=\"font-weight:600 !important;\">Phone:</span> "+ transactionItems .CustomerPhone+ "\r\n            </p>\r\n            <p>\r\n                <span style=\"font-weight:600 !important;\">Email:</span> "+ transactionItems .CustomerEmail+ "\r\n            </p>\r\n\r\n        </div>\r\n        <div style=\"margin-left: 500px;width: 400px;\">\r\n            <table class=\"meta\">\r\n                <tbody>\r\n                    <tr>\r\n                        <th>\r\n                            <span>Invoice #</span>\r\n                        </th>\r\n                        <td>\r\n                            <span> "+ transactionItems .OrignalInvoiceNo+ "</span>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <th><span>Name</span></th>\r\n                        <td><span> "+ transactionItems .CustomerName+ "</span></td>\r\n                    </tr>\r\n                    <tr>\r\n                        <th><span>CSR</span></th>\r\n                        <td><span> "+ transactionItems .CSR+ "</span></td>\r\n                    </tr>\r\n                    <tr>\r\n                        <th><span>Date</span></th>\r\n                        <td style=\"width: 200px;\">\r\n                            <span>\r\n                               "+ transactionItems.PaymentDate + "\r\n                            </span>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <th><span>Due Date</span></th>\r\n                        <td style=\"width: 200px;\">\r\n                            <span>\r\n                                "+ transactionItems .PaymentDate+ "\r\n                            </span>\r\n                        </td>\r\n                    </tr>\r\n                </tbody>\r\n            </table>\r\n        </div>\r\n    </div>\r\n    <br />\r\n    <div class=\"row\">\r\n        <div class=\"col-sm-12\">\r\n            <table class=\"inventory\">\r\n                <thead>\r\n                    <tr>\r\n                        <th style=\"width:25px;\">#</th>\r\n                        <th>Product/Service</th>\r\n                        <th><span><span> Description </span></span></th>\r\n                        <th><span><span> Quantity </span></span></th>\r\n                        <th><span><span> Rate </span></span></th>\r\n                        <th><span><span> Sales Tax % </span></span></th>\r\n                        <th><span><span> Discount % </span></span></th>\r\n                        <th><span><span> Amount </span></span></th>\r\n                    </tr>\r\n                </thead>\r\n                <tbody>\r\n                    <tr #each=\"let item of dataList; let i = index\"+>\r\n                        <td>{{i+1}}</td>\r\n                        <td>{{item.product}}</td>\r\n                        <td>\r\n                            <span>{{item.description}}</span>\r\n                        </td>\r\n                        <td>\r\n                            <span data-prefix=\"\">\r\n                                <span>{{item.quantity}}</span>\r\n                            </span>\r\n                        </td>\r\n                        <td>\r\n                            <span data-prefix=\"\"><span>{{item.rate}}</span>\r\n                            </span>\r\n                        </td>\r\n                        <td>\r\n                            <span data-prefix=\"\"><span>{{item.saleTax}}</span>\r\n                            </span>\r\n                        </td>\r\n                        <td>\r\n                            <span data-prefix=\"\"><span>{{item.discount}}</span>\r\n                            </span>\r\n                        </td>\r\n                        <td>\r\n                            <span data-prefix=\"\"><span class=\"text-left\">{{item.amount}}</span>\r\n                            </span>\r\n                        </td>\r\n                    </tr>\r\n                </tbody>\r\n            </table>\r\n        </div>\r\n    </div>\r\n    <br />\r\n    <div class=\"row\">\r\n        <div style=\"width: 65%;\">\r\n            <h5>Notes</h5>\r\n            <textarea id=\"Note\" [(ngModel)]=\"note\" name=\"Note\" readonly=\"readonly\"\r\n                class=\"border-gray-dark input-mask form-control\" style=\"border: 2px solid gray;\" rows=\"4\"\r\n                cols=\"50\"></textarea>\r\n        </div>\r\n        <div style=\"margin-top: 15px;width: 35%;\">\r\n            <table>\r\n                <tbody>\r\n                    <tr>\r\n                        <td><label style=\"width: 130px;\">Total </label></td>\r\n                        <td><label style=\"width: 100px;\">$ {{total}}</label></td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td><label style=\"width: 130px;\">Tax </label></td>\r\n                        <td><label style=\"width: 100px;\">$ {{totalSaleTax}}</label></td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td><label style=\"width: 130px;\">Amount Received </label></td>\r\n                        <td><label style=\"width: 100px;\"> </label></td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td><label>Balance Due </label></td>\r\n                        <td><label>$ {{total}}<span class=\"AmountToCredit\">0</span></label></td>\r\n                    </tr>\r\n                </tbody>\r\n            </table>\r\n        </div>\r\n    </div>\r\n</div>";
            var htmlString = "<p>invoiceNo</p> " + transactionItems.OrignalInvoiceNo + "<br /> This is the Pdf Template";
            var document = new Document();
            var memoryStream = new MemoryStream();
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();
            using (StringReader sr = new StringReader(htmlString))
            {
                var htmlWorker = new HTMLWorker(document);
                htmlWorker.StartDocument();
                htmlWorker.Parse(sr);
                htmlWorker.EndDocument();
            }
            document.Close();
            var finalMemoryStream = new MemoryStream(memoryStream.ToArray());
            finalMemoryStream.Position = 0;
            return finalMemoryStream;
        }
        public async Task<long> CreatePRInvoice(Invoice invoice, List<Voucher> voucher)
        {
            var invoiceId = await CreateInvoice(invoice);

            return invoiceId;
        }
        public async Task<long> CreateSRInvoice(Invoice invoice, List<Voucher> voucher)
        {
            var invoiceId = await CreateInvoice(invoice);

            return invoiceId;
        }
        public async Task<long> CreateInvoice(Invoice invoice)
        {

            switch (invoice.InvoiceType)
            {
                case InvoiceType.Invoice:
                    invoice.InvoiceStatus = InvoiceStatus.Open;
                    var InvoiceNum = await GetInvoiceNumber(InvoiceType.Invoice);
                    if (string.IsNullOrEmpty(InvoiceNum))
                    {
                        InvoiceNum = "00000001";
                    }
                    invoice.InvoiceNo = "IN-" + InvoiceNum;
                    break;
                case InvoiceType.Purchase_Invoice:
                    invoice.InvoiceStatus = InvoiceStatus.Open;
                    var PIInvoiceNum = await GetInvoiceNumber(InvoiceType.Purchase_Invoice);
                    if (string.IsNullOrEmpty(PIInvoiceNum))
                    {
                        PIInvoiceNum = "00000001";
                    }
                    invoice.InvoiceNo = "PI-" + PIInvoiceNum;
                    break;
                case InvoiceType.Sale_Receipt:
                    invoice.InvoiceStatus = InvoiceStatus.Paid;
                    var SRInvoiceNum = await GetInvoiceNumber(InvoiceType.Sale_Receipt);
                    if (string.IsNullOrEmpty(SRInvoiceNum))
                    {
                        SRInvoiceNum = "00000001";
                    }
                    invoice.InvoiceNo = "SR-" + SRInvoiceNum;
                    break;

                case InvoiceType.Purchase_Receipt:
                    invoice.InvoiceStatus = InvoiceStatus.Closed;
                    var PRInvoiceNum = await GetInvoiceNumber(InvoiceType.Purchase_Receipt);
                    if (string.IsNullOrEmpty(PRInvoiceNum))
                    {
                        PRInvoiceNum = "00000001";
                    }
                    invoice.InvoiceNo = "PR-" + PRInvoiceNum;
                    break;

                case InvoiceType.Check:
                    invoice.InvoiceStatus = InvoiceStatus.Closed;
                    var CKInvoiceNum = await GetInvoiceNumber(InvoiceType.Check);
                    if (string.IsNullOrEmpty(CKInvoiceNum))
                    {
                        CKInvoiceNum = "00000001";
                    }
                    invoice.InvoiceNo = "CK-" + CKInvoiceNum;
                    break;

                case InvoiceType.Estimate:
                    invoice.InvoiceStatus = InvoiceStatus.Closed;
                    var ETInvoiceNum = await GetInvoiceNumber(InvoiceType.Estimate);
                    if (string.IsNullOrEmpty(ETInvoiceNum))
                    {
                        ETInvoiceNum = "00000001";
                    }
                    invoice.InvoiceNo = "ET-" + ETInvoiceNum;
                    break;

                case InvoiceType.Credit_Note:
                    invoice.InvoiceStatus = InvoiceStatus.Closed;
                    var CNInvoiceNum = await GetInvoiceNumber(InvoiceType.Credit_Note);
                    if (string.IsNullOrEmpty(CNInvoiceNum))
                    {
                        CNInvoiceNum = "00000001";
                    }
                    invoice.InvoiceNo = "CN-" + CNInvoiceNum;
                    break;

                case InvoiceType.Receive_Payment:

                    if (invoice.Total > invoice.InvoiceDetails.Sum(x => x.PaidAmount))
                        invoice.InvoiceStatus = InvoiceStatus.Partial;
                    else
                        invoice.InvoiceStatus = InvoiceStatus.Closed;

                    var RPInvoiceNum = await GetInvoiceNumber(InvoiceType.Receive_Payment);
                    if (string.IsNullOrEmpty(RPInvoiceNum))
                    {
                        RPInvoiceNum = "00000001";
                    }
                    invoice.InvoiceNo = "RP-" + RPInvoiceNum;
                    break;

                case InvoiceType.Expense:
                    invoice.InvoiceStatus = InvoiceStatus.Closed;
                    var EXInvoiceNum = await GetInvoiceNumber(InvoiceType.Expense);
                    if (string.IsNullOrEmpty(EXInvoiceNum))
                    {
                        EXInvoiceNum = "00000001";
                    }
                    invoice.InvoiceNo = "EX-" + EXInvoiceNum;
                    break;

                case InvoiceType.Recurring_Invoice:
                    invoice.InvoiceStatus = InvoiceStatus.Open;
                    var RCInvoiceNum = await GetInvoiceNumber(InvoiceType.Recurring_Invoice);
                    if (string.IsNullOrEmpty(RCInvoiceNum))
                    {
                        RCInvoiceNum = "00000001";
                    }
                    invoice.InvoiceNo = "RC-" + RCInvoiceNum;
                    break;

                case InvoiceType.Purchase_Payment:

                    if (invoice.Total > invoice.InvoiceDetails.Sum(x => x.PaidAmount))
                        invoice.InvoiceStatus = InvoiceStatus.Partial;
                    else
                        invoice.InvoiceStatus = InvoiceStatus.Closed;

                    var PPInvoiceNum = await GetInvoiceNumber(InvoiceType.Purchase_Payment);
                    if (string.IsNullOrEmpty(PPInvoiceNum))
                    {
                        PPInvoiceNum = "00000001";
                    }
                    invoice.InvoiceNo = "EV-" + PPInvoiceNum;
                    break;

            }

            var invoiceId = await _invoiceRepository.InsertOrUpdateAndGetIdAsync(invoice);
            await CurrentUnitOfWork.SaveChangesAsync();
            return invoiceId;
        }

        public async Task<long> UpdateInvoiceDetails(Invoice invoice)
        {
            var InvoiceDetail = await _invoiceDetailRepository.GetAll().Where(x => x.InvoiceId == invoice.Id).ToListAsync();
            foreach (var items in InvoiceDetail)
            {
                await _invoiceDetailRepository.DeleteAsync(items);
            }
            invoice.IsActive = true;
            invoice.CreatorUserId = invoice.TenantId;
            invoice.InvoiceStatus = InvoiceStatus.Open;
            await _invoiceRepository.UpdateAsync(invoice);
            foreach (var item in invoice.InvoiceDetails.ToList())
            {
                if (item.IsPaid == true)
                {
                    await _invoiceDetailRepository.DeleteAsync(item);
                }
                else
                {
                    if (item.Id > 0)
                    {
                        await _invoiceDetailRepository.UpdateAsync(item);
                    }
                    else
                    {
                        await _invoiceDetailRepository.InsertAsync(item);
                    }
                }
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            return invoice.Id;
        }
        private async Task UpdatePaidInvoice(Invoice invoice, List<Voucher> vouchers)
        {
            switch (invoice.InvoiceType)
            {
                case InvoiceType.Invoice:
                    decimal customerBalance = await GetCustomerBalance(invoice.RefCustomerId.Value);
                    if (customerBalance < 0)
                    {
                        customerBalance = (-1) * (customerBalance);
                        Invoice _Invoice = new Invoice();
                        _Invoice.IsActive = true;
                        _Invoice.IsDeleted = false;
                        _Invoice.IsPaid = false;

                        _Invoice.PaymentDate = DateTime.Now;
                        _Invoice.RefrenceNo = invoice.RefrenceNo;
                        _Invoice.RefCustomerId = invoice.RefCustomerId;

                        _Invoice.RefDepositToAccountId = 8; // why this value is hard coded?

                        _Invoice.InvoiceType = InvoiceType.Receive_Payment;
                        _Invoice.RefPaymentMethodId = 1; // why this value is hard coded?
                        if (customerBalance > invoice.Total)
                        {
                            _Invoice.Total = invoice.Total;
                        }
                        else
                        {
                            _Invoice.Total = customerBalance;
                        }
                        _Invoice.InvoiceStatus = InvoiceStatus.Closed;
                        _Invoice.InvoiceNo = await GetInvoiceNumber(InvoiceType.Receive_Payment);

                        //creating another invoice
                        await _invoiceRepository.InsertAsync(_Invoice);


                        var _invoiceDetail = new InvoiceDetail();
                        _invoiceDetail.RefPaidInvoiceId = invoice.Id;
                        _invoiceDetail.InvoiceId = _Invoice.Id;
                        _invoiceDetail.PaidAmount = _Invoice.Total;


                        decimal _InvoiceBalance = await GetInvoiceBalance(invoice.Id);
                        if (_InvoiceBalance <= 0)
                        {
                            // set inital created invoice to paid if balance is zero
                            invoice.InvoiceStatus = InvoiceStatus.Paid;
                        }
                        else if (_InvoiceBalance > 0 && _InvoiceBalance < invoice.Total)
                        {
                            // set inital created invoice to open if balance is remaining
                            invoice.InvoiceStatus = InvoiceStatus.Open;
                        }
                        else if (_InvoiceBalance == invoice.Total)
                        {
                            invoice.InvoiceStatus = InvoiceStatus.Open;
                        }


                    }

                    await AddVouchers(invoice, vouchers, invoice.InvoiceDate.Value);
                    break;

                case InvoiceType.Receive_Payment:
                    foreach (var item in invoice.InvoiceDetails)
                    {
                        decimal _InvoiceBalance = await GetInvoiceBalance(item.RefPaidInvoiceId.Value);
                        var paidInvoice = await _invoiceRepository.FirstOrDefaultAsync(a => a.Id == item.RefPaidInvoiceId);

                        decimal _InvoiceTotal = paidInvoice.Total.Value; // to do: refactor after condition testing
                        if (_InvoiceBalance <= 0)
                        {
                            paidInvoice.Total = invoice.Total;
                            paidInvoice.InvoiceStatus = InvoiceStatus.Paid;
                        }
                        else if (_InvoiceBalance > 0 && _InvoiceBalance < _InvoiceTotal)
                        {
                            paidInvoice.Total += invoice.Total;
                            paidInvoice.InvoiceStatus = InvoiceStatus.Partial;
                        }
                        else if (_InvoiceBalance == _InvoiceTotal)
                        {
                            paidInvoice.Total += invoice.Total;
                            paidInvoice.InvoiceStatus = InvoiceStatus.Open;
                        }
                    }
                    break;

                case InvoiceType.Purchase_Payment:
                    foreach (var item in invoice.InvoiceDetails)
                    {
                        decimal _InvoiceBalance = await GetPurchaseInvoiceBalance(item.RefPaidInvoiceId.Value);
                        var paidInvoice = await GetInvoiceAsync(item.RefPaidInvoiceId.Value);

                        decimal _InvoiceTotal = paidInvoice?.Total ?? 0; // to do: condition can be refactor
                        if (_InvoiceBalance <= 0)
                        {
                            paidInvoice.InvoiceStatus = InvoiceStatus.Paid;
                        }
                        else if (_InvoiceBalance > 0 && _InvoiceBalance < _InvoiceTotal)
                        {
                            paidInvoice.InvoiceStatus = InvoiceStatus.Partial;
                        }
                        else if (_InvoiceBalance == _InvoiceTotal)
                        {
                            paidInvoice.InvoiceStatus = InvoiceStatus.Open;
                        }
                    }
                    break;

                case InvoiceType.Sale_Receipt:
                    await AddVouchers(invoice, vouchers, invoice.SaleReceiptDate.Value);
                    break;

                case InvoiceType.Purchase_Receipt:
                    await AddVouchers(invoice, vouchers, invoice.PurchaseReceiptDate.Value);
                    break;

                case InvoiceType.Check:
                    await AddVouchers(invoice, vouchers, invoice.PaymentDate.Value);
                    break;

                case InvoiceType.Purchase_Invoice:
                    await AddVouchers(invoice, vouchers, invoice.InvoiceDate.Value);
                    break;

                case InvoiceType.Recurring_Invoice:
                    await AddVouchers(invoice, vouchers, invoice.InvoiceDate.Value);
                    break;

                case InvoiceType.Credit_Note:
                    await AddVouchers(invoice, vouchers, invoice.CreditNoteDate.Value);
                    break;

                default:
                    if (invoice.InvoiceType == InvoiceType.Purchase_Payment
                        || invoice.InvoiceType == InvoiceType.Receive_Payment
                        || invoice.InvoiceType == InvoiceType.Expense)
                    {
                        await AddVouchers(invoice, vouchers, invoice.PaymentDate.Value);
                    }

                    break;

            }

        }

        private async Task AddVouchers(Invoice invoice, List<Voucher> vouchers, DateTime date)
        {

            foreach (var voucherMaster in vouchers)
            {
                voucherMaster.TransactionDate = invoice.InvoiceDate;
                voucherMaster.VoucherNo = await GetVoucherNumber(voucherMaster.VoucherTypeCode);

                foreach (var item in voucherMaster.VoucherDetails)
                {
                    item.AddDate = DateTime.Now;
                    item.TransactionDate = invoice.SaleReceiptDate;
                    item.Note = GetVoucherDescription(voucherMaster.VoucherTypeCode) + " Against Invoice No:" + invoice.InvoiceNo;
                }

                vouchers.Add(voucherMaster);
                // save changes here
            }
        }

        public async Task<long> AddNewVouchers(long invoiceId, Voucher vouchers)
        {

            try
            {
                var invoiceData = await _invoiceRepository.FirstOrDefaultAsync(x => x.Id == invoiceId);
                vouchers.InvoiceId = invoiceId;
                vouchers.TransactionDate = invoiceData.InvoiceDate;
                var VoucherNo = await GetVoucherNumber(vouchers.VoucherTypeCode);
                vouchers.VoucherNo = await generateVoucherNumber(vouchers.VoucherTypeCode, VoucherNo);

                foreach (var item in vouchers.VoucherDetails)
                {
                    item.AddDate = DateTime.Now;
                    item.TransactionDate = DateTime.Now;
                    item.Note = GetVoucherDescription(vouchers.VoucherTypeCode) + " Against Invoice No:" + invoiceData.InvoiceNo;
                }
                var voucherId = await _voucherRepository.InsertAndGetIdAsync(vouchers);
                return voucherId;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred while creating Voucher", ex.Message);
            }
        }

        public async Task<long> AddNewVouchersSaleReceipt(long invoiceId, List<Voucher> Voucher)
        {

            try
            {
                var invoiceData = await _invoiceRepository.FirstOrDefaultAsync(x => x.Id == invoiceId);
                foreach (var vouchers in Voucher)
                {
                    vouchers.InvoiceId = invoiceId;
                    vouchers.TransactionDate = invoiceData.InvoiceDate;
                    var VoucherNo = await GetVoucherNumber(vouchers.VoucherTypeCode);
                    vouchers.VoucherNo = await generateVoucherNumber(vouchers.VoucherTypeCode, VoucherNo);

                    foreach (var item in vouchers.VoucherDetails)
                    {
                        item.AddDate = DateTime.Now;
                        item.TransactionDate = DateTime.Now;
                        item.Note = GetVoucherDescription(vouchers.VoucherTypeCode) + " Against Invoice No:" + invoiceData.InvoiceNo;
                    }
                    var voucherId = await _voucherRepository.InsertAndGetIdAsync(vouchers);

                    return voucherId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred while creating Voucher", ex.Message);
            }
        }



        public async Task<Invoice> InvoiceByIdGet(EntityDto<long> input)
        {
            var invoice = await _invoiceRepository.GetAll()
                                        .Where(a => a.Id == input.Id)
                                        .FirstOrDefaultAsync();
            var invoiceDetails = await _invoiceDetailRepository.GetAll().Where(x => x.InvoiceId == input.Id).ToListAsync();
            return invoice;
        }

        public async Task<string> generateVoucherNumber(string _Code, string VoucherNo)
        {
            try
            {
                if (_Code == "SV")
                {
                    if (string.IsNullOrEmpty(VoucherNo))
                    {
                        VoucherNo = "00000001";
                    }
                    return "SV-" + VoucherNo;
                }
                else if (_Code == "PV")
                {
                    if (string.IsNullOrEmpty(VoucherNo))
                    {
                        VoucherNo = "00000001";
                    }
                    return "PV-" + VoucherNo;
                }
                else if (_Code == "SR")
                {
                    if (string.IsNullOrEmpty(VoucherNo))
                    {
                        VoucherNo = "00000001";
                    }
                    return "SR-" + VoucherNo;
                }

                else if (_Code == "CR")
                {
                    if (string.IsNullOrEmpty(VoucherNo))
                    {
                        VoucherNo = "00000001";
                    }
                    return "CR-" + VoucherNo;
                }

                else if (_Code == "EV")
                {
                    if (string.IsNullOrEmpty(VoucherNo))
                    {
                        VoucherNo = "00000001";
                    }
                    return "EV-" + VoucherNo;
                }
                else if (_Code == "CK")
                {
                    if (string.IsNullOrEmpty(VoucherNo))
                    {
                        VoucherNo = "00000001";
                    }
                    return "CK-" + VoucherNo;
                }
                else if (_Code == "JV")
                {
                    if (string.IsNullOrEmpty(VoucherNo))
                    {
                        VoucherNo = "00000001";
                    }
                    return "JV-" + VoucherNo;
                }
                return "";
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public string GetVoucherDescription(string _Code)
        {
            try
            {
                string VoucherNote = "";
                if (_Code == "SV")
                {
                    VoucherNote = "Sale Voucher";
                }
                else if (_Code == "PV")
                {
                    VoucherNote = "Payment Voucher";
                }
                else if (_Code == "SR")
                {
                    VoucherNote = "Sale Return Voucher";
                }

                else if (_Code == "CR")
                {
                    VoucherNote = "Cash Receive Voucher";
                }

                else if (_Code == "EV")
                {
                    VoucherNote = "Expense Voucher";
                }
                else if (_Code == "CK")
                {
                    VoucherNote = "Check Voucher";
                }
                return VoucherNote;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public async Task<string> GetVoucherNumber(string voucherTypeCode)
        {
            string voucher = "";
            var tenantId = (int)AbpSession.TenantId;
            voucher = await _invoiceRepository.GetVoucherNumber(voucherTypeCode, tenantId);

            if (voucher != "")
            {
                string numeric = voucher;
                string numericPart = numeric.Substring(3);
                int numericValue = int.Parse(numericPart);
                numericValue++;
                string formattedNumericValue = numericValue.ToString("D8");
                return formattedNumericValue;
            }
            return voucher;
        }

        public async Task<Invoice> GetInvoiceAsync(long id)
        {
            var invoice = await _invoiceRepository.FirstOrDefaultAsync(a => a.Id == id);
            if (invoice == null) throw new UserFriendlyException("Invoice not found");
            return invoice;
        }

        private async Task UpdateInvoice(Invoice invoice, List<Voucher> vouchers)
        {
            try
            {
                var _invoice = await _invoiceRepository.GetAll()
                                        .Where(a => a.Id == invoice.Id)
                                        .Include(a => a.InvoiceDetails)
                                        .FirstOrDefaultAsync();

                _invoice.AmountReceived = invoice.AmountReceived;
                _invoice.CreditNoteDate = invoice.CreditNoteDate;
                _invoice.Email = invoice.Email;
                _invoice.EstimateDate = invoice.EstimateDate;
                _invoice.ExpirationDate = invoice.ExpirationDate;
                _invoice.PurchaseReceiptDate = invoice.PurchaseReceiptDate;
                _invoice.InvoiceDate = invoice.InvoiceDate;
                _invoice.InvoiceDueDate = invoice.InvoiceDueDate;
                _invoice.IsSendLater = invoice.IsSendLater;
                _invoice.Note = invoice.Note;
                _invoice.PaidAmount = invoice.PaidAmount;
                _invoice.PaymentDate = invoice.PaymentDate;
                _invoice.RefrenceNo = invoice.RefrenceNo;
                _invoice.SaleReceiptDate = invoice.SaleReceiptDate;
                _invoice.Duration = invoice.Duration;
                _invoice.DurationId = invoice.DurationId;
                _invoice.Frequency = invoice.Frequency;
                _invoice.FrequencyId = invoice.FrequencyId;
                _invoice.IsRecurring = invoice.IsRecurring;
                _invoice.RefCustomerId = invoice.RefCustomerId;
                _invoice.RefDepositToAccountId = invoice.RefDepositToAccountId;
                _invoice.InvoiceType = invoice.InvoiceType;
                _invoice.RefPaymentTypeId = invoice.RefPaymentTypeId;
                _invoice.RefTermId = invoice.RefTermId;
                _invoice.Total = invoice.Total;
                _invoice.RefSupplierId = invoice.RefSupplierId;
                _invoice.RefCashEquivalentsAccountId = _invoice.RefCashEquivalentsAccountId;
                _invoice.RefPaymentTypeId = invoice.RefPaymentTypeId;
                _invoice.RefCardId = invoice.RefCardId;

                // to do: remove invoice details in batch
                //con.Invoice_Detail.RemoveRange(_InvDetail);

                invoice.InvoiceDetails = invoice.InvoiceDetails;

                if (invoice.InvoiceType == InvoiceType.Sale_Receipt
                    || invoice.InvoiceType == InvoiceType.Check
                    || invoice.InvoiceType == InvoiceType.Purchase_Receipt
                    || invoice.InvoiceType == InvoiceType.Credit_Note
                    || invoice.InvoiceType == InvoiceType.Estimate)
                {
                    _invoice.InvoiceStatus = InvoiceStatus.Closed;
                }

                switch (_invoice.InvoiceType)
                {
                    case InvoiceType.Invoice:
                        {
                            decimal invoiceBalance = await GetInvoiceBalance(invoice.Id);
                            if (invoiceBalance <= 0)
                                _invoice.InvoiceStatus = InvoiceStatus.Paid;
                            else if (invoiceBalance <= invoice.Total)
                                _invoice.InvoiceStatus = InvoiceStatus.Open;

                            await RemoveAndAddNewVouchers(invoice, vouchers, invoice.InvoiceDate.Value);

                            break;
                        }

                    case InvoiceType.Purchase_Invoice:
                        {
                            decimal invoiceBalance = await GetPurchaseInvoiceBalance(invoice.Id);
                            if (invoiceBalance <= 0)
                                _invoice.InvoiceStatus = InvoiceStatus.Paid;
                            else if (invoiceBalance <= invoice.Total)
                                _invoice.InvoiceStatus = InvoiceStatus.Open;

                            await RemoveAndAddNewVouchers(invoice, vouchers, invoice.InvoiceDate.Value);

                        }
                        break;

                    case InvoiceType.Receive_Payment:
                        if (invoice.Total > invoice.InvoiceDetails.Sum(a => a.PaidAmount))
                            _invoice.InvoiceStatus = InvoiceStatus.Partial;
                        else
                            _invoice.InvoiceStatus = InvoiceStatus.Closed;

                        await UpdatePaidInvoice(invoice);

                        break;

                    case InvoiceType.Purchase_Payment:

                        if (invoice.Total > invoice.InvoiceDetails.Sum(a => a.PaidAmount))
                            invoice.InvoiceStatus = InvoiceStatus.Partial;
                        else
                            invoice.InvoiceStatus = InvoiceStatus.Closed;

                        await UpdatePaidInvoice(invoice);
                        break;


                    case InvoiceType.Sale_Receipt:
                        await RemoveAndAddNewVouchers(invoice, vouchers, invoice.SaleReceiptDate.Value);
                        break;

                    case InvoiceType.Purchase_Receipt:
                        await RemoveAndAddNewVouchers(invoice, vouchers, invoice.PurchaseReceiptDate.Value);
                        break;

                    case InvoiceType.Check:
                        await RemoveAndAddNewVouchers(invoice, vouchers, invoice.SaleReceiptDate.Value);
                        break;

                    case InvoiceType.Credit_Note:
                        await RemoveAndAddNewVouchers(invoice, vouchers, invoice.SaleReceiptDate.Value);
                        break;

                    default:
                        if (invoice.InvoiceType == InvoiceType.Purchase_Payment
                            || invoice.InvoiceType == InvoiceType.Receive_Payment
                            || invoice.InvoiceType == InvoiceType.Expense)
                        {
                            await RemoveAndAddNewVouchers(invoice, vouchers, invoice.SaleReceiptDate.Value);
                        }
                        break;
                }


            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("An error occurred while updating invoice", ex.Message);
            }
        }

        private async Task RemoveAndAddNewVouchers(Invoice invoice, List<Voucher> vouchers, DateTime invoiceTypeDate)
        {
            var existingVoucherDetails = await _voucherDetailRepository.GetAll()
                                                .Where(a => a.InvoiceId == invoice.Id)
                                                .ToListAsync();

            //to do: remove exisiting voucher details

            var voucher = await _voucherRepository.FirstOrDefaultAsync(a => a.Id == invoice.Id);

            //to do: uncomment this line when you remove exisiting voucher details
            // await _voucherRepository.DeleteAsync(voucher);

            await AddVouchers(invoice, vouchers, invoiceTypeDate);

        }


        private async Task UpdatePaidInvoice(Invoice invoice)
        {
            foreach (var item in invoice.InvoiceDetails)
            {
                var invoiceBalance = await GetInvoiceBalance(item.RefPaidInvoiceId.Value);
                var paidInvoice = await GetInvoiceAsync(item.RefPaidInvoiceId.Value);

                if (invoiceBalance <= 0)
                {
                    paidInvoice.InvoiceStatus = InvoiceStatus.Paid;
                }
                else if (invoiceBalance < paidInvoice.Total)
                {
                    if ((invoiceBalance - paidInvoice.PaidAmount) < 1)
                        paidInvoice.InvoiceStatus = InvoiceStatus.Paid;
                    else
                        paidInvoice.InvoiceStatus = InvoiceStatus.Partial;
                }
                else if (invoiceBalance == paidInvoice.Total)
                {
                    paidInvoice.InvoiceStatus = InvoiceStatus.Open;
                }
            }
        }

        public async Task<string> GetInvoiceNumber(InvoiceType invoiceType)
        {
            string _InvoiceNum = string.Empty;
            string ResObjJSON = await _invoiceRepository.GetInvoiceNo((AbpSession.TenantId != null ? (int)AbpSession.TenantId : (jobTenantId ?? 0)), (int)invoiceType);

            var ResObjParsed = JsonConvert.DeserializeObject<dynamic>(ResObjJSON);

            if (ResObjParsed.Status == "Successful" && ResObjParsed.InvoiceNum != "")
            {
                string numeric = ResObjParsed.InvoiceNum;
                string numericPart = numeric.Substring(3);
                int numericValue = int.Parse(numericPart);
                numericValue++;
                string formattedNumericValue = numericValue.ToString("D8");
                return formattedNumericValue;
            }
            return _InvoiceNum;

        }

        public async Task<decimal> GetPurchaseInvoiceBalance(long invoiceId)
        {
            decimal _InvoiceBalance = 0;
            string ResObjJSON = await _invoiceRepository.GetInvoiceBalance(invoiceId);

            var ResObjParsed = JsonConvert.DeserializeObject<dynamic>(ResObjJSON);

            if (ResObjParsed.Status == "Successful")
            {
                return ResObjParsed.InvoiceBalance;
            }
            return _InvoiceBalance;

            // to do: create and use sp_Finance_Get_PurchaseInvoiceBalance store procedure which will return purchase invoice balance
            //done by dev-10
        }


        public async Task<decimal> GetCustomerBalance(long customerId)
        {
            try
            {
                decimal _CustomerBalance = 0;
                string ResObjJSON = await _invoiceRepository.GetCustomerBalance(customerId);

                var ResObjParsed = JsonConvert.DeserializeObject<dynamic>(ResObjJSON);

                if (ResObjParsed.Status == "Successful")
                {
                    return ResObjParsed.CustomerBalance;
                }
                return _CustomerBalance;
            }
            catch (Exception)
            {

                throw;
            }
            //to do: create and use procedure "sp_Finance_Report_GetcustomerBalance" to get customer balance
            //done by dev-10
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


        public async Task<List<CustomerTransactionDto>> GetCustomerTransaction(int customerId)
        {
            try
            {
                var res = await _invoiceRepository.GetCustomerTransaction(customerId, (int)AbpSession.TenantId);
                return res;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("An error occurred while getting customer Transactions", ex.Message);
            }
        }
        public async Task<List<CustomerTransactionDto>> GetAllTransactions()
        {
            try
            {
                var res = await _invoiceRepository.GetAllTransactions((int)AbpSession.TenantId);
                return res;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred while getting all Transactions", ex.Message);
            }
        }
        public async Task<int> DeleteTransactionDetail(EntityDto input)
        {
            var invoice = await _invoiceRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            if (invoice != null)
            {
                var invoiceDetail = await _invoiceDetailRepository.GetAll().Where(x => x.InvoiceId == invoice.Id).ToListAsync();
                await _invoiceRepository.DeleteAsync(invoice);
                foreach (var item in invoiceDetail)
                {
                    await _invoiceDetailRepository.DeleteAsync(item);
                }
                await CurrentUnitOfWork.SaveChangesAsync();
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public async Task<List<ReceviedPayment>> GetReceivedPaymentList(EntityDto input)
        {
            try
            {
                List<ReceviedPayment> res = new List<ReceviedPayment>(); // Initialize the list

                if (AbpSession.TenantId.HasValue)
                {
                    var result = await _invoiceRepository.GetReceivedPaymentList((int)AbpSession.TenantId, input);

                    foreach (var item in result)
                    {
                        if (item.OpenBalance > 0)
                        {
                            res.Add(item); // Use Add() method instead of push()
                        }
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<List<ReceviedPayment>> GetPurchasePaymentList(EntityDto input)
        {
            try
            {
                List<ReceviedPayment> res = null;
                if (AbpSession.TenantId.HasValue)
                    res = await _invoiceRepository.GetPurchasePaymentList((int)AbpSession.TenantId, input);
                return res;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<invoiceDetails>> GetInvoiceDetails(EntityDto input)
        {
            try
            {
                List<invoiceDetails> res = null;
                if (AbpSession.TenantId.HasValue)
                    res = await _invoiceRepository.GetInvoiceDetails((int)AbpSession.TenantId, input);
                return res;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<ReceviedPayment>> GetReceivedPaymentDetails(EntityDto input)
        {
            try
            {
                List<ReceviedPayment> res = null;
                if (AbpSession.TenantId.HasValue)
                    res = await _invoiceRepository.GetReceivedPaymentDetails((int)AbpSession.TenantId, input);
                return res;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<PrintDto> GetPrintDetails(long invoiceId)
        {
            try
            {
                PrintDto res = new PrintDto(); // Initialize the PrintDto object
                List<PrintDetail> result = new List<PrintDetail>(); // Initialize the PrintDetails list

                var invoiceData = _invoiceRepository.GetAll().FirstOrDefault(x => x.Id == invoiceId);
                var customerData = _customerRepository.GetAll().FirstOrDefault(x => x.Id == invoiceData.RefCustomerId);
                var custoemrAddress = _addressRepository.GetAll().FirstOrDefault(x => x.CustomerId == customerData.Id);
                var companyData = _companyRepository.GetAll().FirstOrDefault(x => x.TenantId == invoiceData.TenantId);
                var companyAddress = _addressRepository.GetAll().FirstOrDefault(x => x.Id == companyData.AddressId);

                res.Note = invoiceData != null ? invoiceData.Note : "";
                res.RefrenceNo = invoiceData != null ? invoiceData.RefrenceNo : "";
                res.OrignalInvoiceNo = invoiceData != null ? invoiceData.InvoiceNo : "";
                res.PaymentDate = invoiceData != null ? invoiceData.PaymentDate : DateTime.Now;
                res.InvoiceDueDate = invoiceData != null ? invoiceData.InvoiceDueDate : DateTime.Now;
                res.InvoiceDate = invoiceData != null ? invoiceData.InvoiceDate : DateTime.Now;

                res.CustomerName = customerData != null ? customerData.Name : "";
                res.CustomerBussinessName = customerData != null ? customerData.BussinessName : "";
                res.CustomerEmail = customerData != null ? customerData.Email : "";
                res.CustomerAddress = custoemrAddress != null ? custoemrAddress.CompleteAddress : "";
                res.CustomerCity = custoemrAddress != null ? custoemrAddress.City : "";
                res.CustomerCountry = custoemrAddress != null ? custoemrAddress.Country : "";
                res.CustomerPostCode = custoemrAddress != null ? custoemrAddress.PostCode : "";
                res.CustomerState = custoemrAddress != null ? custoemrAddress.State : "";

                res.CompanyName = companyData != null ? companyData.Name : "";
                res.ComAddress = companyAddress != null ? companyAddress.CompleteAddress : "";
                res.ComCity = companyAddress != null ? companyAddress.City : "";
                res.ComCountry = companyAddress != null ? companyAddress.Country : "";
                res.ComPostCode = companyAddress != null ? companyAddress.PostCode : "";
                res.ComState = companyAddress != null ? companyAddress.State : "";

                if (AbpSession.TenantId.HasValue)
                {
                    result = await _invoiceRepository.GetPrintDetails((int)AbpSession.TenantId, invoiceId);
                    res.CSR = result[0].CSR;
                    res.PrintDetails = ObjectMapper.Map<List<PrintDetail>>(result);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<VoucherList>> GetAllVouchers()
        {
            try
            {
                List<VoucherList> res = null;
                if (AbpSession.TenantId.HasValue)
                    res = await _invoiceRepository.GetAllVouchers((int)AbpSession.TenantId);
                return res;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred while getting all Vouchers", ex.Message);
            }
        }

        private async Task<Voucher> AddSpecialVoucherDetails(Invoice invoice, Voucher voucher, long companyId, decimal discountGiven)
        {
            var salesTaxPayableLinkedAccountId = await GetLinkedAccountId((int)LinkedAccountEnum.Sale_Tax_Payables, companyId);
            var customerReceivableLinkedAccountId = await GetLinkedAccountId((int)LinkedAccountEnum.Customers_Receivables, companyId);
            var discountsGivenLinkedAccountId = await GetLinkedAccountId((int)LinkedAccountEnum.Discount_Given, companyId);

            voucher.VoucherDetails.Add(new VoucherDetail()
            {
                RefCompanyId = 0,
                VoucherId = 0,
                RefChartOfAccountId = salesTaxPayableLinkedAccountId,
                SrNo = 1,
                Note = "",
                Dr_Amount = 0,
                BankId = 0,
                Cr_Amount = CalculateSaletax(invoice.InvoiceDetails.ToList()),
                AddDate = DateTime.Today,
                TransactionDate = DateTime.Today,
                IsDeleted = false,
                PartnerId = 0,
                InvoiceId = 0,
                AccountName = "",
            });

            voucher.VoucherDetails.Add(new VoucherDetail()
            {
                RefCompanyId = 0,
                VoucherId = 0,
                RefChartOfAccountId = customerReceivableLinkedAccountId,
                SrNo = 2,
                Note = "",
                Dr_Amount = invoice.Total,
                BankId = 0,
                Cr_Amount = 0,
                AddDate = DateTime.Today,
                TransactionDate = DateTime.Today,
                IsDeleted = false,
                PartnerId = invoice.RefCustomerId,
                InvoiceId = 0,
                AccountName = "",
            });

            voucher.VoucherDetails.Add(new VoucherDetail()
            {
                RefCompanyId = 0,
                VoucherId = 0,
                RefChartOfAccountId = discountsGivenLinkedAccountId,
                SrNo = 2,
                Note = "",
                Dr_Amount = discountGiven,
                BankId = 0,
                Cr_Amount = 0,
                AddDate = DateTime.Today,
                TransactionDate = DateTime.Today,
                IsDeleted = false,
                PartnerId = invoice.RefCustomerId,
                InvoiceId = 0,
                AccountName = "",
            });

            return voucher;

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

        public async Task<long> GetLinkedAccountId(int id, long companyId)
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
