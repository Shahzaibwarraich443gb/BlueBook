using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using AccountingBlueBook.AppServices.Estimate;
using AccountingBlueBook.AppServices.Invoices;
using AccountingBlueBook.AppServices.Invoices.dto;
using AccountingBlueBook.AppServices.ReceivedPayment;
using AccountingBlueBook.AppServices.ReceivedPayment.Dto;
using AccountingBlueBook.AppServices.Recurring_Invoice.dto;
using AccountingBlueBook.AppServices.Recurringinvoice.dto;
using AccountingBlueBook.Common.CommonLookupDto;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AccountingBlueBook.Entities.MainEntities.Customers.Dto;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using AccountingBlueBook.Entities.MainEntities.LinkedAccounts;
using AccountingBlueBook.Entities.MainEntities.Vouchers;
using AccountingBlueBook.Entities.NormalizeEntities;
using AccountingBlueBook.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Recurringinvoice
{
    public class RecurringInvoiceAppService : AccountingBlueBookAppServiceBase, IRecurringInvoiceAppService
    {
        private readonly IRepository<ProductService> _productServiceRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerCard> _companycardRepository;
        private readonly IRepository<RecurringInvoice, long> _recurringInvoiceRepository;
        private readonly IRepository<Voucher, long> _voucherRepository;
        private readonly IRepository<LinkedAccount, long> _linkedAccountRepository;
        private readonly IRepository<VoucherDetail, long> _voucherDetailRepository;
        private readonly IRepository<InvoiceDetail, long> _invoiceDetailRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        private readonly ReceivedPaymentService _receivePaymentService;
        private readonly EstimateAppService _estimateAppService;
        private readonly IInvoiceAppService invocieAppService;
        private readonly InvoiceAppService _invoiceAppService;
        private readonly IRepository<CustomerCard> _customerCardRepository;
        private readonly IReceviedPaymentService _receviedPaymentService;
        private readonly IRepository<Transaction> _transactionRepository;

        public RecurringInvoiceAppService(IRepository<ProductService> productServiceRepository,
                                 IRepository<Customer> customerRepository,
                                 IRepository<Transaction> transactionRepository,
                                 IRepository<Voucher, long> voucherRepository,
                                 IRepository<VoucherDetail, long> voucherDetailRepository,
                                 IRepository<LinkedAccount, long> linkedAccountRepository,
                                 IInvoiceRepository invoiceRepository,
                                 IRepository<CustomerCard> companycardRepository,
                                 IRepository<RecurringInvoice, long> recurringInvoiceRepository,
                                 ReceivedPaymentService receivePaymentService,
                                 EstimateAppService estimateappservice,
                                 IInvoiceAppService invocieAppService,
                                 InvoiceAppService invoiceAppService,
                                 IRepository<CustomerCard> customerCardRepository,
                                 IReceviedPaymentService receviedPaymentService,

        IRepository<InvoiceDetail, long> invoiceDetailRepository)
        {
            _productServiceRepository = productServiceRepository;
            _customerRepository = customerRepository;
            _voucherRepository = voucherRepository;
            _linkedAccountRepository = linkedAccountRepository;
            _voucherDetailRepository = voucherDetailRepository;
            _invoiceDetailRepository = invoiceDetailRepository;
            _invoiceRepository = invoiceRepository;
            _companycardRepository = companycardRepository;
            _recurringInvoiceRepository = recurringInvoiceRepository;
            _receivePaymentService = receivePaymentService;
            _estimateAppService = estimateappservice;
            this.invocieAppService = invocieAppService;
            _invoiceAppService = invoiceAppService;
            _customerCardRepository = customerCardRepository;
            _receviedPaymentService = receviedPaymentService;
            _transactionRepository = transactionRepository;



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

        public async Task SaveCCInfo(CompanyCardDto input)
        {
            CustomerCard comapnyCard = new CustomerCard();
            // comapnyCard.Id =Convert.ToInt16( input.Id);
            comapnyCard.Card_Type = input.Card_Type;
            comapnyCard.Card_Holder_Name = input.Card_Holder_Name;
            comapnyCard.Card_Number = input.Card_Number;
            comapnyCard.Exp_Date = input.Exp_Date;
            comapnyCard.CCV_No = input.CCV_No;
            comapnyCard.ref_CustomerId = input.ref_CustomerId;


            await _companycardRepository.InsertAsync(comapnyCard);
            //await CurrentUnitOfWork.SaveChangesAsync();


        }

        public async Task<List<CompanyCardDto>> GetAllCards(long customerId)
        {
            var companyCardList = await _companycardRepository.GetAll().Where(x => x.ref_CustomerId == customerId && x.IsDeleted == false).Select(x => new CompanyCardDto
            {
                Id = Convert.ToInt16(x.Id),
                Card_Type = x.Card_Type,
                Card_Holder_Name = x.Card_Holder_Name,
                Card_Number = x.Card_Number,
                Exp_Date = x.Exp_Date,
                CCV_No = x.CCV_No,
                ref_CustomerId = x.ref_CustomerId

            }).ToListAsync();
            return companyCardList;
        }

        public async Task DeleteCompanyCardByCustomerIdAndCardId(long Id)
        {
            var companyCard = await _companycardRepository.GetAll().Where(x => x.Id == Id).FirstOrDefaultAsync();

            if (companyCard != null)
            {
                await _companycardRepository.DeleteAsync(companyCard);

            }
        }





        public async Task SaveRecurringData(RecurringInvoiceDto input)
        {
            try
            {
                var recurringInvoice = ObjectMapper.Map<RecurringInvoice>(input);

                await _recurringInvoiceRepository.InsertAsync(recurringInvoice);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task AddRecurringInvoice(RecurringInvoice invoice)
        {
            var invoiceObj = JsonConvert.DeserializeObject<CreateInvoiceDto>(invoice.InvoiceData);
            invoiceObj.TenantId = invoice.TenantId;
            var invoiceId = await _invoiceAppService.SaveInvoice(invoiceObj);

            var invoiceRes = await _invoiceRepository.FirstOrDefaultAsync(invoiceId);

            //receiving payment
            SaveReceivedPayment receivedPayment = new();
            receivedPayment.Emails = new List<string> { _customerRepository.FirstOrDefault((int)invoiceObj.Invoice.RefCustomerID).Email ?? "" };
            receivedPayment.PaymentDate = DateTime.Now;


          

            receivedPayment.ReceivedPayments = new();

            foreach (var data in invoiceObj.Invoice.InvoiceDetails)
            {
                ReceviedPayment recObj = new();
                recObj.InvoiceID = invoiceRes.Id;
                recObj.InvoiceNo = invoiceRes.InvoiceNo;
                recObj.IsCheck = true;
                recObj.OpenBalance = invoiceRes.Total;
                recObj.PaidAmount = invoiceRes.Total;
                recObj.InvoiceDueDate = invoiceRes.InvoiceDueDate;

                receivedPayment.ReceivedPayments.Add(recObj);
            }
            receivedPayment.RefCustomerID = invoiceObj.Invoice.RefCustomerID;
            receivedPayment.RefInvoiceType = 5;
            receivedPayment.RefPaymentMethodID = 1;
            receivedPayment.Total = invoiceRes.Total;
            receivedPayment.InvoiceNo = invoiceRes.InvoiceNo;


            await _receviedPaymentService.SaveReceivedPayment(receivedPayment, invoice.TenantId);
        }

        [UnitOfWork]
        public async Task RecurringInvoiceJob()
        {
            try
            {
                var recurringInvoices = await _recurringInvoiceRepository.GetAllListAsync();

                foreach (var invoice in recurringInvoices)
                {
                    var customer = await _customerRepository.FirstOrDefaultAsync((int)invoice.CustomerId);

                    if (customer != null)
                    {
                        bool execute = false;

                        //checking if the frequency matches current date
                        switch (invoice.FrequencyId)
                        {
                            case 1:
                                if(invoice.FrequencyAnnualDate == null)
                                {
                                    break;
                                }

                                var annualDT = invoice.FrequencyAnnualDate ?? DateTime.Now;

                                if(annualDT.Day == DateTime.Now.Day && annualDT.Month == DateTime.Now.Month)
                                {
                                    execute = true;
                                }
                                break;
                            
                            
                            case 2:
                                if(invoice.FrequencyMonth == DateTime.Now.Month && invoice.LastExecution.Year < DateTime.Now.Year)
                                {
                                    execute = true;
                                }
                                break;
                            
                            
                            case 3:

                                DateTime dateTime = DateTime.Now; // Replace with your DateTime object

                                DayOfWeek startDayOfWeek = DayOfWeek.Sunday; // Specify the start day of the week

                                int weekNumber = (int)startDayOfWeek - (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(dateTime) + 1;
                                
                                if (weekNumber <= 0)
                                {
                                    weekNumber += 7;
                                }

                                
                                if (invoice.FrequencyWeek == weekNumber)
                                {
                                    execute = true;
                                }
                                break;
                            
                            
                            case 4:
                                if(invoice.FrequencyCustomDate == null)
                                {
                                    break;
                                }
                                var customDT = invoice.FrequencyCustomDate ?? DateTime.Now;
                                if(customDT.Date == DateTime.Now.Date)
                                {
                                    execute = true;
                                }
                                break;
                        }

                        if(execute == false)
                        {
                            continue;
                        }

                        //adding recurring invoice
                        switch (invoice.DurationId)
                        {
                            case 1:
                                await AddRecurringInvoice(invoice);
                                break;

                            case 2:
                                if(invoice.ExecutedAmount == null)
                                {
                                    invoice.ExecutedAmount = 0;
                                }

                                if(invoice.DurationAmount > invoice.ExecutedAmount)
                                {
                                   await AddRecurringInvoice(invoice);
                                    invoice.ExecutedAmount++;
                                }
                                break;

                            case 3:
                                if((invoice.DurationDateTill ?? DateTime.Now).Date > DateTime.Now.Date)
                                {
                                   await AddRecurringInvoice(invoice);
                                }
                                break;
                        }


                        //recieve payment


                       



                        //ChargeCardDto ChargeCard = new();
                        //var InvoiceObj = JsonConvert.DeserializeObject<CreateInvoiceDto>(invoice.InvoiceData);
                        //var Cardobj = await  _customerCardRepository.FirstOrDefaultAsync(invoice.CustomerCardId);
                        //ChargeCard.Amount = InvoiceObj.Invoice.Total;
                        //ChargeCard.CardNumber = Cardobj.Card_Number;
                        //ChargeCard.CustomerEmail = customer.Email;
                        //ChargeCard.CardType = "amex";
                        //ChargeCard.TenantId = invoice.TenantId;

                        //await _receivePaymentService.SaveChargeCard(ChargeCard);



                        //send mail
                        if (invoice.SendMail == true)
                        {

                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }



        public async Task<Invoice> CalculateIssueDueDate(Invoice model)
        {
            try
            {


                var paymentTermList = await _estimateAppService.GetPaymentTermTermLists();
                int _termDays = 0;
                if (paymentTermList.Count() > 0)
                {
                    var singlePaymentTerm = paymentTermList.Where(x => x.Id == model.RefTermId).Select(x => x.Days);
                    _termDays = Convert.ToInt32(singlePaymentTerm);
                }
                //else
                //{
                //    _termDays = 0;

                //}
                // .Where(c => c.ID == model.ref_TermID).Select(c => c.Days).FirstOrDefault().HasValue ? new BL_Invoice().Get_Payment_Term_List().Where(c => c.ID == model.ref_TermID).Select(c => c.Days).FirstOrDefault().Value : 0;
                int FrequencyId = model.FrequencyId.HasValue ? model.FrequencyId.Value : 0;
                //mamnage the issue date
                DateTime CurrentDate = DateTime.Now;
                switch (model.FrequencyId)
                {
                    case 1:
                        model.InvoiceDueDate = model.InvoiceDate.Value.AddDays(_termDays);
                        break;
                    case 2:

                        if (CurrentDate.Day > Convert.ToInt32(model.Frequency))
                        {
                            model.InvoiceDate = CurrentDate.AddMonths(1);
                            model.InvoiceDate = new DateTime(model.InvoiceDate.Value.Year, model.InvoiceDate.Value.Month, Convert.ToInt32(model.Frequency));
                        }
                        else
                        {
                            model.InvoiceDate = new DateTime(model.InvoiceDate.Value.Year, model.InvoiceDate.Value.Month, Convert.ToInt32(model.Frequency));
                        }
                        model.InvoiceDueDate = model.InvoiceDate.Value.AddDays(_termDays);
                        break;
                    case 3:
                        int day = Convert.ToInt32(model.InvoiceDate.Value.DayOfWeek);
                        int Weekday = 7;
                        if (day > Convert.ToInt32(model.Frequency))
                        {
                            int DayCal = Weekday - day;
                            model.InvoiceDate = model.InvoiceDate.Value.AddDays(DayCal + (Convert.ToInt32(model.Frequency)));
                        }
                        else if (day == Convert.ToInt32(model.Frequency))
                        {

                        }
                        else
                        {
                            int DayCal = Convert.ToInt32(model.Frequency) - day;
                            model.InvoiceDate = model.InvoiceDate.Value.AddDays(DayCal);
                        }
                        model.InvoiceDueDate = model.InvoiceDate.Value.AddDays(_termDays);
                        break;
                    case 4:
                        model.InvoiceDate = model.InvoiceDate.Value.AddDays(Convert.ToInt32(model.Frequency));
                        model.InvoiceDueDate = model.InvoiceDate.Value.AddDays(_termDays);
                        break;


                }
                return model;


                //    #region Old Code
                //    //if (model.FrequencyId == Convert.ToInt32(RecurringInvoiceFrequencyType.Yearly))
                //    //{
                //    //    //model.InvoiceDueDate = Convert.ToDateTime(model.InvoiceDate).AddYears(1);
                //    //    model.InvoiceDueDate = Convert.ToDateTime(model.InvoiceDate).AddDays(_termsDays);
                //    //}
                //    //else if (model.FrequencyId == Convert.ToInt32(RecurringInvoiceFrequencyType.Monthly))
                //    //{
                //    //    string date = Convert.ToString(model.InvoiceDate.Value.Day);
                //    //    if (Convert.ToInt32(date) > Frequency)
                //    //    {
                //    //        if (model.InvoiceDate.Value.Month + Convert.ToInt32(DefaultDays.Day) == Convert.ToInt32(DefaultDays.Day))
                //    //            model.InvoiceDate = Convert.ToDateTime((model.InvoiceDate.Value.Month + Convert.ToInt32(DefaultDays.Day)) + "/" + Frequency + "/" + (model.InvoiceDate.Value.Year + Convert.ToInt32(DefaultDays.Day)));
                //    //        else
                //    //            model.InvoiceDate = Convert.ToDateTime((model.InvoiceDate.Value.Month + Convert.ToInt32(DefaultDays.Day)) + "/" + Frequency + "/" + (model.InvoiceDate.Value.Year));
                //    //    }
                //    //    else
                //    //    {
                //    //        if (model.InvoiceDate.Value.Month + Convert.ToInt32(DefaultDays.Day) == Convert.ToInt32(DefaultDays.Day))
                //    //            model.InvoiceDate = Convert.ToDateTime(model.InvoiceDate.Value.Month + "/" + Frequency + "/" + (model.InvoiceDate.Value.Year + Convert.ToInt32(DefaultDays.Day)));
                //    //        else
                //    //            model.InvoiceDate = Convert.ToDateTime(model.InvoiceDate.Value.Month + "/" + Frequency + "/" + (model.InvoiceDate.Value.Year));
                //    //    }
                //    //    //model.InvoiceDueDate = model.InvoiceDate.Value.AddMonths(1);
                //    //    model.InvoiceDueDate = model.InvoiceDate.Value.AddDays(_termsDays);
                //    //}
                //    //else if (model.FrequencyId == Convert.ToInt32(RecurringInvoiceFrequencyType.Weekly))
                //    //{
                //    //    int day = Convert.ToInt32(model.InvoiceDate.Value.DayOfWeek);
                //    //    if (day == Frequency)
                //    //    {
                //    //        model.InvoiceDate = Convert.ToDateTime(model.InvoiceDate.Value.Month + "/" + model.InvoiceDate.Value.Day + "/" + model.InvoiceDate.Value.Year);
                //    //    }
                //    //    else if (Frequency > day)
                //    //    {
                //    //        int days = Frequency - day;
                //    //        DateTime newIssueDate = new DateTime();
                //    //        newIssueDate = DateTime.Now.AddDays(days);
                //    //        model.InvoiceDate = Convert.ToDateTime(newIssueDate.Month + "/" + newIssueDate.Day + "/" + newIssueDate.Year);
                //    //    }
                //    //    else
                //    //    {
                //    //        int weekDays = 7;
                //    //        int days = weekDays - day;
                //    //        DateTime newIssueDate = new DateTime();
                //    //        newIssueDate = DateTime.Now.AddDays(days + Frequency);
                //    //        model.InvoiceDate = Convert.ToDateTime(newIssueDate.Month + "/" + newIssueDate.Day + "/" + newIssueDate.Year);
                //    //    }
                //    //    //model.InvoiceDueDate = model.InvoiceDate.Value.AddDays(7);
                //    //    model.InvoiceDueDate = model.InvoiceDate.Value.AddDays(_termsDays);
                //    //}
                //    //else
                //    //{
                //    //    //model.InvoiceDueDate = model.InvoiceDate.Value.AddDays(Convert.ToInt32(model.Frequency));
                //    //    model.InvoiceDueDate = model.InvoiceDate.Value.AddDays(_termsDays);
                //    //}

                //    #endregion
                //    return model;
                //}
            }
            catch (Exception ex)
            {
                //ExceptionHandler.LogMessage(ex, "Controller: RecurringInvoice, Action: CalculateIssueDueDate");
                return null;
            }
        }





        public async Task<Customer> Getcustomerbyid(long id)
        {

            var a = await _customerRepository.GetAsync((int)id);
            return a;

        }

        public async Task AutoChargeInvoice(Invoice invoice, ICollection<InvoiceDetail> _InvoiceDatail)
        {


            ChargeCardDto vm = new ChargeCardDto();
            var cc = _companycardRepository.Get(invoice.RefCardId);
            var customer = await Getcustomerbyid((long)invoice.RefCustomerId);

            vm.CustomerEmail = customer.Email;

            vm.Amount = invoice.Total.Value;
            vm.CardHolderName = cc.Card_Holder_Name;
            vm.CardNumber = cc.Card_Number;

            vm.CardType = cc.Card_Type;
            vm.CCVNo = cc.CCV_No;

            //vm.Duration = ??;
            vm.ExpDate = cc.Exp_Date;
            if (!string.IsNullOrEmpty(vm.CardNumber))
            {
                if (vm.Amount > 0)
                {



                    bool IsAmountCharged = await _receivePaymentService.SaveChargeCard(vm);
                    if (IsAmountCharged == true)
                    {
                        //string InvoicejsonString = String.Format("{{ \"ref_CustomerID\": \"{0}\", \"ref_PaymentMethodID\": \"{1}\", \"ref_DepositToAccountID\": \"{2}\", \"PaymentDate\": \"{3}\", \"InvoiceID\": \"{4}\",\"Total\": \"{5}\",\"IsSendLater\": \"{6}\",\"Email\": \"{7}\",\"ref_InvoiceType\": \"{8}\",\"RefrenceNo\": \"{9}\",\"Note\": \"{10}\" ,\"ref_CompanyID\": \"{11}\" }}", invoice.ref_CustomerID.Value, 40037, 13, DateTime.Now.ToString("MM/dd/yyyy"), 0, invoice.Total.Value, false, vm.CustomerEmail, 5, "", "", invoice.ref_CompanyID);
                        //string InvoiceDetailJsonString = String.Format("[{{ \"IsChecked\": \"{0}\", \"ref_PaidInvoiceID\": \"{1}\", \"PaidAmount\": \"{2}\"  }}]", true, invoice.InvoiceID, invoice.Total.Value);
                        var aaa = SaveRecurringPaymentOfRecurringInvoice(invoice);// ask what is ref_deposittoAccountId
                    }
                    else
                    {
                        //  return "Invalid Card";
                    }
                }
            }





        }
        public async Task SaveRecurringPaymentOfRecurringInvoice(Invoice input)
        {



            //ObjectMapper.Map<Invoice>(input.Invoice);

            Invoice invoice = new Invoice
            {

                RefCustomerId = input.RefCustomerId,
                RefTermId = input.RefTermId,
                Email = input.Email.Count() > 0 ? string.Join(",", input.Email) : null,
                TenantId = AbpSession.TenantId == null ? 1 : (int)AbpSession.TenantId,
                RefCompanyId = AbpSession.TenantId == null ? 1 : (int)AbpSession.TenantId,
                InvoiceType = input.InvoiceType,
                //InvoiceStatus = (InvoiceStatus?)input.Invoice.RefInvoiceStatus,
                FrequencyId = input.FrequencyId,
                Duration = input.Duration,
                DurationId = input.DurationId,
                Total = input.Total,
                RefPaymentTypeId = Convert.ToInt32(input.RefPaymentTypeId),
                RefCardId = Convert.ToInt32(input.RefCardId),
                Note = input.Note,
                IsRecurring = true,
                Frequency = input.Frequency,



            };





            List<InvoiceDetail> invoiceDetailList = input.InvoiceDetails.Select(data => new InvoiceDetail
            {
                Id = 0,
                Amount = data.Amount,
                Discount = data.Discount,
                Quantity = data.Quantity,
                Rate = data.Rate,
                RefProducId = data.RefProducId,
                SaleTax = data.SaleTax,
                TenantId = 1,
                RefCustomerId = input.RefCustomerId,
                PaidAmount = data.PaidAmount,
                Description = data.Description,
                IsPaid = false
            }).ToList();
            invoice.InvoiceDetails = invoiceDetailList;


            var voucher = new Voucher();
            decimal discountGiven = 0;


            var salesVoucherMaster = new Voucher();
            var productIds = input.InvoiceDetails.Select(a => a.RefProducId);

            var incomeAccoundIds = await _productServiceRepository.GetAll().Where(a => productIds.Any(x => x == a.Id)).Select(a => a.Id).ToListAsync();

            // company id is replaced with tenant id
            var companyId = AbpSession.TenantId == null ? 1 : (int)AbpSession.TenantId;

            var linkedAccountId = await GetLinkedAccountId((int)LinkedAccountEnum.Sales_Revenue, companyId);



            foreach (var item in input.InvoiceDetails)
            {
                var voucherDetail = new VoucherDetail()
                {
                    RefCompanyId = 0,
                    VoucherId = 0,
                    RefChartOfAccountId = incomeAccoundIds.Count(a => a == item.RefProducId) > 0 ? item.RefProducId : linkedAccountId,
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
            //if ( Save_InvoiceRecurring (_InvoiceObj, _InvoiceDetailListObj, _VouchersList))
            {

                long invoiceId = await _invoiceAppService.CreateInvoice(invoice);
                //new BL_AppLog().SaveLog(_InvoiceObj.ref_CustomerID, "Payment Added Of Invoice By Recurring", DateTime.Now, "Added", _InvoiceObj.ref_CompanyID, _InvoiceObj.AddedByID);


                var invoiceData = await _invoiceRepository.FirstOrDefaultAsync(x => x.Id == invoiceId);
                var transaction = new Entities.MainEntities.Transaction
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

                if (invoice.IsSendLater == false)
                {
                    //new ReceivePaymentController().EmailReceivePayment(_InvoiceObj.InvoiceID, _InvoiceObj.Email, _InvoiceObj.Email);
                }


            }




        }



        public async Task<List<KeyValuePair<long, string>>> GetMonthlyFrequency()
        {

            var IstMonthlyFrequency = new List<KeyValuePair<long, string>>();
            if (IstMonthlyFrequency != null)
            {
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(1, "1st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(2, "2st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(3, "3st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(4, "4st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(5, "5st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(6, "6st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(7, "7st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(8, "8st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(9, "9st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(10, "10st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(11, "11st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(12, "12st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(13, "13st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(14, "14st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(15, "15st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(16, "16st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(17, "17st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(18, "18st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(19, "19st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(20, "20st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(21, "21st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(22, "22st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(23, "23st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(24, "24st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(25, "25st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(26, "26st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(27, "27st"));
                IstMonthlyFrequency.Add(new KeyValuePair<long, string>(28, "28st"));



                return IstMonthlyFrequency;
            }



            return null;

        }

        public async Task<List<KeyValuePair<long, string>>> GetWeeklyFrequency()
        {
            var IstWeeklyFrequency = new List<KeyValuePair<long, string>>();
            if (IstWeeklyFrequency != null)
            {


                IstWeeklyFrequency.Add(new KeyValuePair<long, string>(1, "Sunday"));

                IstWeeklyFrequency.Add(new KeyValuePair<long, string>(2, "Monday"));

                IstWeeklyFrequency.Add(new KeyValuePair<long, string>(3, "Tuesday"));

                IstWeeklyFrequency.Add(new KeyValuePair<long, string>(4, "Wednesday"));

                IstWeeklyFrequency.Add(new KeyValuePair<long, string>(5, "Thursday"));
                IstWeeklyFrequency.Add(new KeyValuePair<long, string>(6, "Friday"));
                IstWeeklyFrequency.Add(new KeyValuePair<long, string>(7, "Saturday"));



                return IstWeeklyFrequency;


            }
            return null;

        }





        private async Task CreateInvoice(Invoice invoice, Voucher voucher)
        {

            try
            {
                invoice.IsActive = true;
                //invoice.IsPaid = true;

                await _invoiceAppService.CreateInvoice(invoice);

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("An error occurred while creating invoice", ex.Message);
            }
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
            await _invoiceRepository.GetVoucherNumber(voucherTypeCode, tenantId);
            //to do: create and use procedure "Finance_SpGetVouvherNO" to get voucher number
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
            string ResObjJSON = await _invoiceRepository.GetInvoiceNo(AbpSession.TenantId != null ? (int)AbpSession.TenantId : 1, (int)invoiceType);

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
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<ReceviedPayment>> GetReceivedPaymentList(EntityDto input)
        {
            try
            {
                List<ReceviedPayment> res = null;
                if (AbpSession.TenantId.HasValue)
                    res = await _invoiceRepository.GetReceivedPaymentList((int)AbpSession.TenantId, input);
                return res;
            }
            catch (Exception ex)
            {

                throw;
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
