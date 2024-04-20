using Abp.Domain.Repositories;
using Abp.UI;
using AccountingBlueBook.AppServices.Invoices;
using AccountingBlueBook.AppServices.ReceivedPayment.Dto;
using AccountingBlueBook.Authorization.Users;
using AccountingBlueBook.Authorization.Users.Dto;
using AccountingBlueBook.CardConnectConfiguration;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AccountingBlueBook.Entities.MainEntities.Customers.Dto;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using AccountingBlueBook.Entities.MainEntities.Vouchers;
using AccountingBlueBook.Enums;
using AccountingBlueBook.Net.Emailing;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.html.simpleparser;

namespace AccountingBlueBook.AppServices.ReceivedPayment
{
    public class ReceivedPaymentService : AccountingBlueBookAppServiceBase, IReceviedPaymentService
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Merchant> _merchantRepository;
        private readonly IRepository<Customer> _customerRepository;
        private static string ENDPOINT = "";
        private int tenantId = 0;
        private readonly IRepository<Voucher, long> _voucherRepository;
        private readonly IRepository<VoucherDetail, long> _voucherDetailRepository;
        private readonly IRepository<CardTransaction> _cardTransactionRepository;
        private readonly IRepository<Invoice, long> _invoiceRepository;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly InvoiceAppService _invoiceService;
        private readonly IRepository<InvoiceDetail, long> _invoiceDetailRepository;
        private readonly IEmailAppServices _emailAppService;
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        //private readonly IPaidFormRepository _iPaidFormRepository;
        private enum OPERATIONS { GET, PUT, POST, DELETE };
        public ReceivedPaymentService(IRepository<Transaction> transactionRepository, IRepository<CardTransaction> cardTransactionRepository,
            IRepository<Customer> customerRepository, IRepository<Invoice, long> invoiceRepository, InvoiceAppService invoiceService, IRepository<Company> companyRepository,
            IRepository<Merchant> merchantRepository, IRepository<InvoiceDetail, long> invoiceDetailRepository, IRepository<Voucher, long> voucherRepository,
            IRepository<VoucherDetail, long> voucherDetailRepository, IEmailAppServices emailAppService, IEmailTemplateProvider emailTemplateProvider
            //,IPaidFormRepository iPaidFormRepository
            )
        {
            _emailAppService = emailAppService;
            _emailTemplateProvider = emailTemplateProvider;
            _invoiceDetailRepository = invoiceDetailRepository;
            //  iPaidFormRepository = _iPaidFormRepository;
            _voucherRepository = voucherRepository;
            _voucherDetailRepository = voucherDetailRepository;
            _customerRepository = customerRepository;
            _merchantRepository = merchantRepository;
            _companyRepository = companyRepository;
            _cardTransactionRepository = cardTransactionRepository;
            _invoiceRepository = invoiceRepository;
            _transactionRepository = transactionRepository;
            _invoiceService = invoiceService;
        }

        public async Task<long> SaveReceivedPayment(SaveReceivedPayment input, int tenantId = 0)
        {
            try
            {
                this.tenantId = tenantId;
                long invoiceId = 0;
                var customer = await _customerRepository.FirstOrDefaultAsync(x => x.Id == input.RefCustomerID);
                ChargeCardDto card_obj = new ChargeCardDto();
                if (input.ChargeCard != null)
                {
                    if (!String.IsNullOrWhiteSpace(input.ChargeCard.CardNumber))
                    {
                        input.ChargeCard.Amount = input.Total;
                        input.ChargeCard.CustomerEmail = customer.Email;
                        var IsAmountCharged = await SaveChargeCard(input.ChargeCard);
                        if (IsAmountCharged == true)
                        {
                            invoiceId = await InvoiceDetails(input, customer);
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        invoiceId = await InvoiceDetails(input, customer);
                    }
                }
                else
                {
                    invoiceId = await InvoiceDetails(input, customer);
                }

                return invoiceId;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred while creating payment received", ex.Message);
            };
        }

        public async Task<long> InvoiceDetails(SaveReceivedPayment input, Customer customer)
        {
            var companyId = _companyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;
            var invoice = new Invoice
            {
                Id = input.Id == null ? 0 : (long)input.Id,
                TenantId = AbpSession.TenantId != null ? (int)AbpSession.TenantId : tenantId,
                Email = input.Emails.Count() > 0 ? string.Join(",", input.Emails) : null,
                RefCompanyId = companyId,
                RefCustomerId = customer.Id,
                RefPaymentMethodId = input.RefPaymentMethodID,
                RefDepositToAccountId = input.RefDepositToAccountId,
                PaymentDate = input.PaymentDate,
                RefrenceNo = input.ReferenceNo,
                InvoiceDate = System.DateTime.Now,
                Total = input.Total,
                PaidAmount = input.PaidAmount,
                InvoiceType = (InvoiceType?)input.RefInvoiceType,
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
                RefCustomerId = customer.Id,
                IsPaid = true
            }).ToList();

            invoice.InvoiceDetails = List;

            var voucher = new Voucher()
            {
                VoucherNo = "",
                VoucherTypeCode = "CR",
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
                RefChartOfAccountId = 82, // Customers_Receivables 
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

            await _voucherRepository.InsertAndGetIdAsync(voucher);


            if (invoice.Id > 0)
            {
                //await UpdateRPAmount(invoice.Id);
                await UpdateInvoiceDetails(invoice, input);
            }
            else
            {
                long invoiceId = await _invoiceService.CreateInvoices(invoice, voucher);

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
        public async Task UpdateRPAmount(long invoiceId)
        {
            var invoiceDetail = await _invoiceDetailRepository.GetAll().Where(x => x.InvoiceId == invoiceId).ToListAsync();
            foreach (var item in invoiceDetail)
            {
                var partialinvoice = await _invoiceRepository.FirstOrDefaultAsync(x => x.Id == item.RefPaidInvoiceId);
                partialinvoice.PaidAmount = item.PaidAmount;
                await _invoiceRepository.UpdateAsync(partialinvoice);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
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

        public async Task<bool> SaveChargeCard(ChargeCardDto carddata)
        {
            try
            {
                int tenantId = carddata.TenantId ?? 0;
                if (AbpSession.TenantId != null)
                {
                    tenantId = (int)AbpSession.TenantId;
                }
                var company = await _companyRepository.GetAll().Where(x => x.TenantId == tenantId).FirstOrDefaultAsync();
                var merchant = await _merchantRepository.FirstOrDefaultAsync(x => x.Id == company.MerchantId);
                string responseString = "";
                string jsonString = "";
                string respstat = "";
                bool status = false;
                string cardWithSterik = carddata.CardNumber;
                int? MerchantId = company.MerchantId;
                string merchantname = merchant.Name;
                long TenantId = (long)tenantId;
                string returnstr = string.Empty;
                string MerchRef = "";
                string TransType = "purchase";
                string Method = "credit_card";
                string CardType = carddata.CardType;
                if (CardType == "amex") CardType = "american express";
                decimal amount = (decimal)carddata.Amount;
                var toEmail = carddata.CustomerEmail;// "iftikharhamza158@gmail.com"; //"dev05.karma@gmail.com";

                if (merchantname == "Card Connect")
                {
                    JObject response = CCauthTransaction(carddata, company, merchant.CCUN, merchant.CCPWD, out string ReqCreated);
                    if (response != null)
                        responseString = response.ToString();
                    else responseString = "";
                    jsonString = ReqCreated;

                    CardTransaction details = new CardTransaction
                    {
                        Location = "SubscriptonController",
                        TimeStamp = DateTime.Today,
                        UserEmail = carddata.CustomerEmail,
                        PreTransaction = jsonString,
                        ProTransaction = responseString
                    };

                    await _cardTransactionRepository.InsertAsync(details);

                    // respstat = (string)response.GetValue("respstat");
                    //  string respstext = (string)response.GetValue("resptext");
                    //  decimal paidAmount = (decimal)response.GetValue("amount");
                    var respstext = "A";
                    if (respstext == "A")
                    {
                        string body = "Your card " + cardWithSterik + " has been charged the amount $" + amount + " for E-Filing";
                        sendPaymentEmail("E-File Payment Successful", body, TenantId, cardWithSterik, amount, toEmail);
                        // await _iPaidFormRepository.DoSomeWork(paidAmount, carddata.Id, merchant.Id, carddata);
                        return true;
                    }
                    else
                    {
                        string body = "Your card " + cardWithSterik + " have been declined while charging the amount $" + amount + " for E-Filing" + " due to " + respstext;
                        sendPaymentEmail("E-File Payment Declined", body, TenantId, cardWithSterik, amount, toEmail);
                        throw new UserFriendlyException("Payment Declined", "Payment Declined due to " + respstext);
                        return false;

                    }
                }
                else if (merchantname == "Pay Easy")
                {
                    decimal Amount1 = amount;
                    int Amount = Convert.ToInt32(Amount1);

                    jsonString = String.Format("{{ \"merchant_ref\": \"{0}\", \"transaction_type\": \"{1}\", \"method\": \"{2}\", \"amount\": \"{3}\", \"currency_code\": \"USD\", \"credit_card\": {{\"type\": \"{4}\", \"cardholder_name\": \"{5}\", \"card_number\": \"{6}\", \"exp_date\": \"{7}\", \"cvv\": \"{8}\" }} }}", MerchRef, TransType, Method, Amount, CardType, carddata.CardHolderName, carddata.CardNumber, carddata.ExpDate, carddata.CCVNo);

                    string token = merchant.Token;
                    string apiSecret = merchant.APISecretKey;
                    string apiKey = merchant.APIKey;
                    string nonce = PayeezyDevSDK.nonce();
                    string timestamp = PayeezyDevSDK.UnixTime().ToString();

                    string authorization = PayeezyDevSDK.CalculateHMAC(apiKey, nonce, timestamp, token, jsonString, apiSecret);
                    string urlType = "Standard";
                    string arg2 = "";
                    string url = PayeezyDevSDK.SetURL(urlType, arg2);

                    HttpWebRequest webRequest = PayeezyDevSDK.Create_Request(url, timestamp, nonce, token, apiKey, authorization, jsonString);

                    CardTransactionDetails details = new CardTransactionDetails();
                    details.Location = "SubscriptonController";
                    details.TimeStamp = DateTime.Today;
                    //   details.UserEmail = user.Email;
                    details.UserEmail = toEmail;//carddata.CustomerEmail;
                    details.PreTransaction = jsonString;
                    try
                    {
                        StreamWriter writer = null;
                        writer = new StreamWriter(webRequest.GetRequestStream());
                        writer.Write(jsonString);
                        writer.Close();
                        using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                        {
                            using (StreamReader responseStream = new StreamReader(webResponse.GetResponseStream()))
                            {
                                responseString = responseStream.ReadToEnd();
                                details.ProTransaction = responseString;
                                dynamic RespPackage = JsonConvert.DeserializeObject(responseString);
                            }
                        }
                    }
                    catch (WebException ex)
                    {
                        if (ex.Response != null)
                        {
                            using (HttpWebResponse errorResponse = (HttpWebResponse)ex.Response)
                            {
                                using (StreamReader reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    string remoteEx = reader.ReadToEnd();
                                    details.ProTransaction = remoteEx;
                                    responseString = remoteEx;

                                }
                            }
                        }
                    }
                    JavaScriptSerializer serializer = new JavaScriptSerializer();

                    Transaction_Result_ViewModel result = new Transaction_Result_ViewModel();
                    try
                    {
                        result = serializer.Deserialize<Transaction_Result_ViewModel>(responseString);

                        if (result.validation_status != null)
                        {
                            if (result.validation_status.Trim().ToLower() == "success" && result.transaction_status.Trim().ToLower() == "approved" && result.transaction_status.Trim().ToLower() != "" && result.transaction_status.Trim().ToLower() != " " && result.transaction_status.Trim().ToLower() != "declined")
                            {
                                //status = true;
                                try
                                {
                                    string body = "Your card " + cardWithSterik + " has been charged the amount $" + amount + " for E-Filing";
                                    sendPaymentEmail("E-File Payment Successful", body, TenantId, cardWithSterik, amount, toEmail);
                                    //await _iPaidFormRepository.DoSomeWork(amount, cardId, TenantId, merchantObj.Id, cartList);
                                    return true;

                                }
                                catch (Exception)
                                {

                                }

                            }
                            else
                            {
                                try
                                {
                                    string body = "Your card " + cardWithSterik + " have been declined while charging the amount $" + amount + " for E-Filing";
                                    sendPaymentEmail("E-File Payment Declined", body, TenantId, cardWithSterik, amount, toEmail);
                                    throw new UserFriendlyException("Payment Declined", "Payment Declined");
                                    return false;
                                }
                                catch (Exception)
                                {

                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string body = "Your card " + cardWithSterik + " have been declined while charging the amount $" + amount + " for E-Filing";
                        sendPaymentEmail("E-File Payment Declined", body, TenantId, cardWithSterik, amount, toEmail);
                        throw new UserFriendlyException("Payment Declined", "Payment Declined. ");
                        return false;
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred while creating charge card", ex.Message);
            }
        }
        public static JObject CCauthTransaction(ChargeCardDto carddata, Company company, string userName, string password, out string ReqCreated)
        {
            string CardNumber = carddata.CardNumber.Replace(" ", "");
            ENDPOINT = "https://fts-uat.cardconnect.com:6443/cardconnect/rest";    //For Production
            //ENDPOINT = "https://fts-uat.cardconnect.com/cardconnect/rest/";     // For Testing

            JObject request = new JObject();
            request.Add("merchid", company.tk);
            request.Add("amount", carddata.Amount);
            request.Add("expiry", carddata.ExpDate);
            request.Add("account", CardNumber);
            request.Add("name", carddata.CardHolderName);
            request.Add("cvv2", carddata.CCVNo);
            request.Add("currency", "USD");
            request.Add("capture", "Y");

            ReqCreated = request.ToString();
            CardConnectRestClient client = new CardConnectRestClient(ENDPOINT, userName, password);
            JObject response = client.authorizeTransaction(request);
            return response;
        }
        private void sendPaymentEmail(string subject, string body, long TenantId, string cardWithSterik, decimal amount, string toEmail)
        {
            _emailAppService.SendMail(new EmailsDto() { Subject = subject, Body = GetEmailBody(subject, body, TenantId).ToString(), ToEmail = toEmail });
        }
        private StringBuilder GetEmailBody(string subject, string body, long? tenantId)
        {
            //var emailTemplate = new StringBuilder("this is email Body");
            var tId = tenantId;
            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(Convert.ToInt32(tenantId)));
            emailTemplate.Replace("{EMAIL_TITLE}", "E-File E-WorkForce AccountingBlueBook");
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", subject);
            emailTemplate.Replace("{EMAIL_BODY}", @$"<p>" + body + "</p> <br/>");

            return emailTemplate;
        }
        public async Task EmailReceivePayment(CustomerTransactionDto transactionItems, string toEmail)
        {
            var TenantId = AbpSession.TenantId;
            string subject = "Thank you for your payment";
            StringBuilder emailbody = new StringBuilder();
            emailbody.Append("<p>Dear Customer,</p>");
            emailbody.AppendLine();
            emailbody.Append("<p>Please check the attachment for the payment.</p>");
            emailbody.AppendLine();
            emailbody.Append("<p>Regards,</p>");
            emailbody.Append(transactionItems.Company);

            var attachmentStreams = new List<Tuple<string, MemoryStream>>();
            var pdfContent = GeneratePdf(transactionItems);
            attachmentStreams.Add(new Tuple<string, MemoryStream>("Payment.pdf", pdfContent));
            await _emailAppService.SendMail(new EmailsDto()
            {
                Subject = subject,
                Body = emailbody.ToString(),
                ToEmail = toEmail,
                Streams = attachmentStreams
            });
        }

        private static MemoryStream GeneratePdf(CustomerTransactionDto transactionItems)
        {
            //var htmlString = "<div #printArea class=\"print-area\" style=\"margin: 25px;\">\r\n    <br />\r\n    <div class=\"row\">\r\n        <div class=\"col-sm-12\">\r\n            <p class=\"uppercase\" style=\"font-weight:600\">" + transactionItems.Company + "</p>\r\n            <p class=\"uppercase\">\r\n                " + transactionItems.ComAddress + "\r\n            </p>\r\n            <p class=\"uppercase\">" + transactionItems.ComCity + " , " + transactionItems.ComState + " " + transactionItems.ComPostCode + "</p>\r\n            <p class=\"uppercase\">" + transactionItems.ComCountry + "</p>\r\n            <p>\r\n                <span style=\"font-weight:600 !important;\">Phone:</span> " + transactionItems.ComPhone + "\r\n            </p>\r\n            <p>\r\n            </p>\r\n            <div><span style=\"font-weight:600 !important;\">Email:</span> " + transactionItems.ComEmail + "</div>\r\n            <p></p>\r\n        </div>\r\n    </div>\r\n    <br />\r\n    <div class=\"row\">\r\n        <div class=\"col-sm-12\">\r\n            <p class=\"uppercase\" style=\"color:#53abc6;text-align:left; font-size:24px\">Received Payment</p>\r\n        </div>\r\n    </div>\r\n    <br />\r\n    <div class=\"row\">\r\n        <div class=\"col-sm-7\">\r\n            <h2 style=\"font-weight:600 !important;\">Recipient</h2>\r\n            <p>\r\n                " + transactionItems.AddedBy + "\r\n            </p>\r\n            <p style=\"width:500px;\">\r\n                test address\r\n            </p>\r\n            <p>\r\n            </p>\r\n            <p> </p>\r\n            <p>\r\n                <span style=\"font-weight:600 !important;\">Phone:</span> +920000000000\r\n            </p>\r\n            <p>\r\n                <span style=\"font-weight:600 !important;\">Email:</span> " + transactionItems.Email + "\r\n            </p>\r\n\r\n        </div>\r\n        <div style=\"margin-left: 500px;width: 400px;\">\r\n            <table class=\"meta\">\r\n                <tbody>\r\n                    <tr>\r\n                        <th>\r\n                            <span>Invoice #</span>\r\n                        </th>\r\n                        <td>\r\n                            <span> " + transactionItems.OrignalInvoiceNo + "</span>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <th><span>Name</span></th>\r\n                        <td><span> " + transactionItems.AddedBy + "</span></td>\r\n                    </tr>\r\n                    <tr>\r\n                        <th><span>Date</span></th>\r\n                        <td style=\"width: 200px;\">\r\n                            <span>\r\n                                " + transactionItems.PaymentDate + "\r\n                            </span>\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <tr>\r\n                        <th><span>Reference No</span></th>\r\n                        <td>\r\n                            <span>\r\n                               " + transactionItems.RefrenceNo + "\r\n                            </span>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <th><span>Amount Received</span></th>\r\n                        <td>\r\n                            <span>\r\n                                $\r\n                            </span>\r\n                            <span>\r\n                                " + transactionItems.Total + "\r\n                            </span>\r\n                        </td>\r\n                    </tr>\r\n                </tbody>\r\n            </table>\r\n        </div>\r\n    </div>\r\n    <br />\r\n    <div class=\"row\">\r\n        <div class=\"col-sm-12\">\r\n            <table class=\"inventory\">\r\n                <thead>\r\n                    <tr>\r\n                        <th style=\"width:25px\">#</th>\r\n                        <th>Product/Service</th>\r\n                        <th><span><span> InvoiceNo </span></span></th>\r\n                        <th><span><span> Invoice Due Date </span></span></th>\r\n                        <th><span><span> Original Amount </span></span></th>\r\n                        <th><span><span> Received Amount </span></span></th>\r\n                    </tr>\r\n                </thead>\r\n                <tbody>\r\n                    <tr>\r\n                        <td>1</td>\r\n                        <td>" + transactionItems.Product +"</td>\r\n                        <td>\r\n                            <span>" + transactionItems.OrignalInvoiceNo + "</span>\r\n                        </td>\r\n                        <td>\r\n                            <span data-prefix=\"\">\r\n                                <span>" + transactionItems.InvoiceDate + "</span>\r\n                            </span>\r\n                        </td>\r\n                        <td>\r\n                            <span data-prefix=\"\"><span>" + transactionItems.Total + "</span>\r\n                            </span>\r\n                        </td>\r\n                        <td>\r\n                            <span data-prefix=\"\"><span class=\"text-left\">" + transactionItems.Balance + "</span>\r\n                            </span>\r\n                        </td>\r\n                    </tr>\r\n                </tbody>\r\n            </table>\r\n        </div>\r\n    </div>\r\n    <br />\r\n    <div class=\"row\">\r\n        <div style=\"width: 65%;\">\r\n            <h5>Notes</h5>\r\n            <textarea id=\"Notes\" name=\"Notes\" readonly=\"readonly\" class=\"border-gray-dark input-mask form-control\"\r\n                style=\"border: 2px solid gray;\" rows=\"4\" cols=\"50\"></textarea>\r\n        </div>\r\n        <div style=\"margin-top: 15px;width: 35%;\">\r\n            <table>\r\n                <tbody>\r\n                    <tr>\r\n                        <td><label style=\"width: 130px;\">Amount Paid:</label></td>\r\n                        <td><label style=\"width: 100px;\">$ " + transactionItems.Total + "</label></td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td><label>Balance:</label></td>\r\n                        <td><label>$ <span class=\"AmountToCredit\">0</span></label></td>\r\n                    </tr>\r\n                </tbody>\r\n            </table>\r\n        </div>\r\n    </div>\r\n</div>";
            var htmlString = "<div #printArea class=\"print-area\"   <br />\r\n    <div class=\"row\">\r\n  <div class=\"col-sm-12\">\r\n  <p style=\"margin-top: 5px !important;\" class=\"uppercase\" style=\"font-weight:600;border:2px solid black\">" + transactionItems.Company + "</p>\r\n            <p style=\"margin-top: 5px !important;\" class=\"uppercase\">\r\n                " + transactionItems.ComAddress + "\r\n            </p>\r\n            <p style=\"margin-top: 5px !important;\" class=\"uppercase\">" + transactionItems.ComCity + " , " + transactionItems.ComState + "\r\n                " + transactionItems.ComPostCode + "</p>\r\n            <p style=\"margin-top: 5px !important;\" class=\"uppercase\">" + transactionItems.ComCountry + "</p>\r\n            <p style=\"margin-top: 5px !important;\">\r\n                <span style=\"font-weight:600 !important;\">Phone:</span> " + transactionItems.ComPhone + "\r\n            </p>\r\n            <p style=\"margin-top: 5px !important;\">\r\n            </p>\r\n            <div><span style=\"font-weight:600 !important;\">Email:</span> " + transactionItems.ComEmail + "</div>\r\n            <p style=\"margin-top: 5px !important;\"></p>\r\n        </div>\r\n    </div>\r\n    <br />\r\n    <div class=\"row\">\r\n        <div class=\"col-sm-12\">\r\n            <p style=\"margin-top: 5px !important;\" class=\"uppercase\"\r\n                style=\"color:#53abc6;text-align:left; font-size:24px\">Received Payment</p>\r\n        </div>\r\n    </div>\r\n    <br />\r\n    <div class=\"row\">\r\n        <div class=\"col-sm-7\">\r\n            <h2 style=\"font-weight:600 !important;\">Recipient</h2>\r\n            <p style=\"margin-top: 5px !important;\">\r\n                " + transactionItems.AddedBy + "\r\n            </p>\r\n            <p style=\"margin-top: 5px !important;\" style=\"width:500px;\">\r\n                test address\r\n            </p>\r\n            <p style=\"margin-top: 5px !important;\">\r\n            </p>\r\n            <p style=\"margin-top: 5px !important;\"> </p>\r\n            <p style=\"margin-top: 5px !important;\">\r\n                <span style=\"font-weight:600 !important;\">Phone:</span> +920000000000\r\n            </p>\r\n            <p style=\"margin-top: 5px !important;\">\r\n                <span style=\"font-weight:600 !important;\">Email:</span> " + transactionItems.Email + "\r\n            </p>\r\n\r\n        </div>\r\n        <div style=\"margin-left: 500px;width: 400px;\">\r\n            <table style=\"text-indent: 19px;\r\n  border-collapse: initial;\" class=\"meta\">\r\n                <tbody style=\"border: 1px solid\">\r\n                    <tr style=\"width: 180px;\r\n  border: 1px solid;\r\n  text-align: center; border: 1px solid black\">\r\n                        <th style=\"background: #cbe3eb;\r\n                        width: 170px;\r\n                        padding: 0px 5px 0px 5px;\r\n                        border: 2px solid #939393;\r\n                        color: #53abc6;\">\r\n                            <span>Invoice #</span>\r\n                        </th>\r\n                        <td style=\"border: 1px solid;\">\r\n                            <span> " + transactionItems.OrignalInvoiceNo + "</span>\r\n                        </td>\r\n                    </tr>\r\n                    <tr style=\"width: 180px;\r\n  border: 1px solid;\r\n  text-align: center; border: 1px solid\">\r\n                        <th style=\"background: #cbe3eb;\r\n                        width: 170px;\r\n                        padding: 0px 5px 0px 5px;\r\n                        border: 2px solid #939393;\r\n                        color: #53abc6;\"><span>Name</span></th>\r\n                        <td style=\"border: 1px solid;\"><span> " + transactionItems.AddedBy + "</span></td>\r\n                    </tr>\r\n                    <tr style=\"width: 180px;\r\n  border: 1px solid;\r\n  text-align: center; border: 1px solid\">\r\n                        <th style=\"background: #cbe3eb;\r\n                        width: 170px;\r\n                        padding: 0px 5px 0px 5px;\r\n                        border: 2px solid #939393;\r\n                        color: #53abc6;\"><span>Date</span></th>\r\n                        <td style=\"border: 1px solid;width: 200px;\">\r\n                            <span>\r\n                                " + transactionItems.InvoiceDate + "\r\n                            </span>\r\n                        </td>\r\n                    </tr>\r\n\r\n                    <tr style=\"width: 180px;\r\n  border: 1px solid;\r\n  text-align: center; border: 1px solid\">\r\n                        <th style=\"background: #cbe3eb;\r\n                        width: 170px;\r\n                        padding: 0px 5px 0px 5px;\r\n                        border: 2px solid #939393;\r\n                        color: #53abc6;\"><span>Reference No</span></th>\r\n                        <td style=\"border: 1px solid;\">\r\n                            <span>\r\n                                " + transactionItems.RefrenceNo + "\r\n                            </span>\r\n                        </td>\r\n                    </tr>\r\n                    <tr style=\"width: 180px;\r\n  border: 1px solid;\r\n  text-align: center; border: 1px solid\">\r\n                        <th style=\"background: #cbe3eb;\r\n                        width: 170px;\r\n                        padding: 0px 5px 0px 5px;\r\n                        border: 2px solid #939393;\r\n                        color: #53abc6;\"><span>Amount Received</span></th>\r\n                        <td style=\"border: 1px solid;\">\r\n                            <span>\r\n                                $\r\n                            </span>\r\n                            <span>\r\n                                " + transactionItems.Total + "\r\n                            </span>\r\n                        </td>\r\n                    </tr>\r\n                </tbody>\r\n            </table>\r\n        </div>\r\n    </div>\r\n    <br />\r\n    <div class=\"row\">\r\n        <div class=\"col-sm-12\">\r\n            <table style=\"text-indent: 19px;\r\n  border-collapse: initial;\" class=\"inventory\">\r\n                <thead>\r\n                    <tr style=\"width: 180px;\r\n  border: 1px solid;\r\n  text-align: center; border: 1px solid\">\r\n                        <th style=\"background: #cbe3eb;\r\n                        width: 170px;\r\n                        padding: 0px 5px 0px 5px;\r\n                        border: 2px solid #939393;\r\n                        color: #53abc6;\" style=\"width:25px;border: 1px solid;\">#</th>\r\n                        <th style=\"background: #cbe3eb;\r\n                        width: 170px;\r\n                        padding: 0px 5px 0px 5px;\r\n                        border: 2px solid #939393;\r\n                        color: #53abc6;\">Product/Service</th>\r\n                        <th style=\"background: #cbe3eb;\r\n                        width: 170px;\r\n                        padding: 0px 5px 0px 5px;\r\n                        border: 2px solid #939393;\r\n                        color: #53abc6;\"><span><span> InvoiceNo </span></span></th>\r\n                        <th style=\"background: #cbe3eb;\r\n                        width: 170px;\r\n                        padding: 0px 5px 0px 5px;\r\n                        border: 2px solid #939393;\r\n                        color: #53abc6;\"><span><span> Invoice Due Date </span></span></th>\r\n                        <th style=\"background: #cbe3eb;\r\n                        width: 170px;\r\n                        padding: 0px 5px 0px 5px;\r\n                        border: 2px solid #939393;\r\n                        color: #53abc6;\"><span><span> Original Amount </span></span></th>\r\n                        <th style=\"background: #cbe3eb;\r\n                        width: 170px;\r\n                        padding: 0px 5px 0px 5px;\r\n                        border: 2px solid #939393;\r\n                        color: #53abc6;\"><span><span> Received Amount </span></span></th>\r\n                    </tr>\r\n                </thead>\r\n                <tbody style=\"border: 1px solid\">\r\n                    <tr style=\"width: 180px;\r\n  border: 1px solid;\r\n  text-align: center; border: 1px solid\">\r\n                        <td style=\"border: 1px solid blue;\">1</td>\r\n                        <td style=\"border: 1px solid;\">" + transactionItems.Product + "</td>\r\n                        <td style=\"border: 1px solid;\">\r\n                            <span>" + transactionItems.OrignalInvoiceNo + "</span>\r\n                        </td>\r\n                        <td style=\"border: 1px solid;\">\r\n                            <span data-prefix=\"\">\r\n                                <span>" + transactionItems.InvoiceDate + "</span>\r\n                            </span>\r\n                        </td>\r\n                        <td style=\"border: 1px solid;\">\r\n                            <span data-prefix=\"\"><span>" + transactionItems.Total + "</span>\r\n                            </span>\r\n                        </td>\r\n                        <td style=\"border: 1px solid;\">\r\n                            <span data-prefix=\"\"><span class=\"text-left\">" + transactionItems.Balance + "</span>\r\n                            </span>\r\n                        </td>\r\n                    </tr>\r\n                </tbody>\r\n            </table>\r\n        </div>\r\n    </div>\r\n    <br />\r\n    <div class=\"row\">\r\n        <div style=\"width: 65%;\">\r\n            <h5>Notes</h5>\r\n            <textarea id=\"Notes\" name=\"Notes\" readonly=\"readonly\" class=\"border-gray-dark input-mask form-control\"\r\n                style=\"border: 2px solid gray;\" rows=\"4\" cols=\"50\"></textarea>\r\n        </div>\r\n        <div style=\"margin-top: 15px;width: 35%;\">\r\n            <table style=\"text-indent: 19px;\r\n  border-collapse: initial;\">\r\n                <tbody style=\"border: 1px solid\">\r\n                    <tr style=\"width: 180px;\r\n  border: 1px solid;\r\n  text-align: center; border: 1px solid\">\r\n                        <td style=\"border: 1px solid;\"><label style=\"width: 130px;\">Amount Paid:</label></td>\r\n                        <td style=\"border: 1px solid;\"><label style=\"width: 100px;\">$ " + transactionItems.Total + "</label></td>\r\n                    </tr>\r\n                    <tr style=\"width: 180px;\r\n  border: 1px solid;\r\n  text-align: center; border: 1px solid\">\r\n                        <td style=\"border: 1px solid;\"><label>Balance:</label></td>\r\n                        <td style=\"border: 1px solid;\"><label>$ <span class=\"AmountToCredit\">0</span></label></td>\r\n                    </tr>\r\n                </tbody>\r\n            </table>\r\n        </div>\r\n    </div>\r\n</div>";

            //string Company = transactionItems.Company;
            //string CompanyAddress = transactionItems.ComAddress;
            //string meetingAgenda = "Discuss Project Updates";
            //string htmlTemplateFilePath = "../AccountingBlueBook.Core/Net/Emailing/EmailTemplates/InvoiceTemplate.html";
            //string htmlTemplate = System.IO.File.ReadAllText(htmlTemplateFilePath);
            //string filledTemplate = string.Format(htmlTemplate, Company, CompanyAddress, meetingAgenda);

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

            //var document = new Document();
            //MemoryStream pdfStream = new MemoryStream();
            //PdfWriter.GetInstance(document, pdfStream);

            //document.Open();
            //document.Add(new Paragraph("Transaction Details"));
            //document.Close();

            //var stream = new MemoryStream(pdfStream.ToArray());
            //stream.Position = 0;

            //return stream;
        }

        public void MyAction(CustomerTransactionDto transactionItems)
        {
            string meetingName = "Sample Meeting";
            string meetingAgenda = "Discuss Project Updates"; // Read the HTML template file string
            string htmlTemplateFilePath = "AccountingBlueBook.Net.Emailing.EmailTemplates.InvoiceTemplate.html";
            string htmlTemplate = System.IO.File.ReadAllText(htmlTemplateFilePath); // Fill in the placeholders with values using string formatting
            string filledTemplate = string.Format(htmlTemplate, meetingName, meetingAgenda);
            //return Content(filledTemplate, "text/html");
        }

        public class Transaction_Result_ViewModel
        {
            public string correlation_id { get; set; }
            public System.Runtime.InteropServices.JavaScript.JSType.Error Error { get; set; }
            public string transaction_status { get; set; }
            public string validation_status { get; set; }
            public string transaction_type { get; set; }
            public string method { get; set; }
            public string amount { get; set; }
            public string currency { get; set; }

            public string transaction_tag { get; set; }
            public CardTransaction card { get; set; }
            public string bank_resp_code { get; set; }
            public string bank_message { get; set; }
            public string gateway_resp_code { get; set; }
            public string gateway_message { get; set; }

            public string transaction_id { get; set; }

        }
        public class CardTransactionDetails
        {
            public long ID { get; set; }
            public string PreTransaction { get; set; }
            public string Location { get; set; }
            public string UserEmail { get; set; }
            public string ProTransaction { get; set; }
            public Nullable<System.DateTime> TimeStamp { get; set; }
        }

        public class PayeezyDevSDK
        {
            public static double UnixTime()
            {
                try
                {
                    /*
                        This function will create a timestamp showing total milliseconds from Unix Epoch 
                    */
                    DateTime date = DateTime.UtcNow;
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    TimeSpan span = (date - epoch);
                    double time = Math.Truncate(span.TotalMilliseconds);
                    return time;
                }
                catch (Exception ex)
                {
                    //ExceptionHandler.LogMessage(ex, "PayeezyDevSDK, UnixTime");
                    return 0;
                }
            }

            public static string nonce()
            {
                /*
                 * This function creates a random sequence for use with the HMAC Calculation and
                 * HTTP Headers
                 */
                try
                {
                    Random random = new Random();
                    string nonce = (random.Next(1000000000, 2147483647)).ToString();
                    return nonce;
                }
                catch (Exception ex)
                {
                    //ExceptionHandler.LogMessage(ex, "PayeezyDevSDK, nonce");
                    return "";
                }
            }

            public static string CalculateHMAC(string apiKey, string nonce, string timestamp, string token, string jsonString, string secret)
            {
                try
                {
                    string hashData = apiKey + nonce + timestamp + token + jsonString;
                    HMAC hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
                    //byte[] dataBytes = Encoding.UTF8.GetBytes(hashData);
                    byte[] hmac2Hex = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(hashData));
                    string hex = BitConverter.ToString(hmac2Hex);
                    hex = hex.Replace("-", "").ToLower();
                    byte[] hexArray = Encoding.UTF8.GetBytes(hex);
                    string base64Hash = Convert.ToBase64String(hexArray);
                    return base64Hash;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            public static string SetURL(string ttype, string transID)
            {
                try
                {
                    string host = "api.payeezy.com"; //replace with "api.payeezy.com" for Production
                    string apiVersion = "v1";
                    string transType = "";
                    switch (ttype)
                    {
                        case "Standard":
                            transType = "transactions";
                            break;
                        case "PostToken":
                            transType = "transactions/tokens";
                            break;
                        case "GetToken":
                            transType = "securitytokens";
                            break;
                    }
                    string url = String.Format("https://{0}/{1}/{2}/{3}", host, apiVersion, transType, transID);
                    return url;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            public static HttpWebRequest Create_Request(string url, string timestamp, string nonce, string token, string apiKey, string authorization, string jsonString)
            {
                try
                {
                    /* Below we are going to create our webRequest */
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                    webRequest.Method = "POST";
                    webRequest.Accept = "*/*";
                    webRequest.Headers.Add("timestamp", timestamp);
                    webRequest.Headers.Add("nonce", nonce);
                    webRequest.Headers.Add("token", token);
                    webRequest.Headers.Add("apikey", apiKey);
                    webRequest.Headers.Add("Authorization", authorization);
                    webRequest.ContentLength = jsonString.Length;
                    webRequest.ContentType = "application/json";
                    return webRequest;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
