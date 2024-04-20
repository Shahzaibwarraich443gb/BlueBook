using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using AccountingBlueBook.AppServices.Invoices;
using AccountingBlueBook.AppServices.JournalVoucher.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities.Vouchers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.JournalVoucher
{
    public class JournalVoucherService : AccountingBlueBookAppServiceBase, IJournalVoucherService
    {
        private readonly IRepository<Voucher, long> _voucherRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly InvoiceAppService _invoiceService;
        public JournalVoucherService(IRepository<Company> companyRepository, InvoiceAppService invoiceService,
            IRepository<Voucher, long> voucherRepository)
        {
            _voucherRepository = voucherRepository; ;
            _companyRepository = companyRepository;
            _invoiceService = invoiceService;
        }
        public async Task<string> SaveJournalVoucher(SaveJournalVouchers input)
        {
            try
            {
                var company = _companyRepository.GetAll().FirstOrDefault(x => x.TenantId == AbpSession.TenantId);
                var VoucherNo = await _invoiceService.GetVoucherNumber("JV");
                var generatedVoucherNo = await _invoiceService.generateVoucherNumber("JV", VoucherNo);
                Voucher _voucher = new Voucher()
                {
                    VoucherNo = generatedVoucherNo,
                    VoucherTypeCode = "JV",
                    AddDate = DateTime.Today,
                    TransactionDate = input.InvoiceDate, //DateTime.Today,
                    LastModificationTime = DateTime.Today,
                    CreatorUserId = AbpSession.UserId,
                    LastModifierUserId = AbpSession.UserId,
                    PaymentMode = 0,
                    IsDeleted = false,
                    PaymentType = 0,
                    //CompanyID = company.Id
                };
                int index = 0;
                foreach (var item in input.VoucherDetails)
                {
                    VoucherDetail _voucherDetail = new VoucherDetail()
                    {
                        VoucherFk = _voucher,
                        Dr_Amount = (decimal)item.DAmount,
                        Cr_Amount = (decimal)item.CAmount,
                        RefChartOfAccountId = item.ChartOfAccountID,
                        //RefCustomerId = item.RefCustomerId,
                        RefCompanyId = _voucher.TenantId,
                        SrNo = index + 1,  // Increment SrNo by 1 in each loop iteration
                        PartnerId = item.RefCustomerId,
                        AddDate = DateTime.Today,
                        TransactionDate = DateTime.Today,
                        LastModificationTime = DateTime.Today,
                        CreatorUserId = AbpSession.UserId,
                        LastModifierUserId = AbpSession.UserId,
                        IsDeleted = false,
                        InvoiceId = 0,
                        Note = item.Description,
                        AccountName = item.Name
                    };
                    _voucher.VoucherDetails.Add(_voucherDetail);
                    index++; 
                }
                var voucherId = await _voucherRepository.InsertOrUpdateAndGetIdAsync(_voucher);
                return "success";
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred while creating Journal Voucher", ex.Message);
            };
        }
        public async Task<int> DeleteVoucherDetail(EntityDto input)
        {
            var invoice = await _voucherRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            if (invoice != null)
            {
                await _voucherRepository.DeleteAsync(invoice);
                await CurrentUnitOfWork.SaveChangesAsync();
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
