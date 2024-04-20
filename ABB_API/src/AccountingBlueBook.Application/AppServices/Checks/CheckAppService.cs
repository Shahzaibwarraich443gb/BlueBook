using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AccountingBlueBook.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Checks
{
    public class CheckAppService : AccountingBlueBookAppServiceBase, ICheckAppService
    {
        private readonly IRepository<Check, long> _CheckRepository;
        private readonly IRepository<Company> _CompanyRepository;
        private readonly IRepository<CheckAccountDetail, long> _CheckAccountRepository;
        private readonly IRepository<CheckProductDetail, long> _CheckProductRespository;
        private readonly IRepository<CheckSetup, long> _CheckSetupRepository;
        private readonly IRepository<Customer> _CustomerRepository;
        private readonly IRepository<Bank> _BankRepository;
        private readonly IRepository<Vendor> _VendorRepository;

        public CheckAppService(
            IRepository<Check, long> checkRepository,
            IRepository<Company> companyRepository,
            IRepository<Customer> customerRepository,
            IRepository<Bank> bankRepository,
            IRepository<CheckAccountDetail, long> checkAccountRepository,
            IRepository<CheckProductDetail, long> checkProductRespository,
            IRepository<CheckSetup, long> checkSetupRepository,
            IRepository<Vendor> vendorRepository
            )
        {
            _CheckRepository = checkRepository;
            _CompanyRepository = companyRepository;
            _CustomerRepository = customerRepository;
            _BankRepository = bankRepository;
            _CheckAccountRepository = checkAccountRepository;
            _CheckProductRespository = checkProductRespository;
            _CheckSetupRepository = checkSetupRepository;
            _VendorRepository = vendorRepository;
        }

        public async Task<List<CheckDto>> GetCheck(bool showDeleted = false, int bankId = 0)
        {

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete))
            {

                var Company = _CompanyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId);
                

                // Load the related entities using Join and GroupJoin
                var checkDtos = await (from Check in _CheckRepository.GetAll()
                                       where Check.CompanyId == Company.Id && (showDeleted || Check.IsDeleted == false) && (bankId > 0 ? Check.BankId == bankId : true)
                                       join Bank in _BankRepository.GetAll() on Check.BankId equals Bank.Id into BankGroup
                                       from Bank in BankGroup.DefaultIfEmpty()
                                       select new CheckDto
                                       {
                                           Id = Check.Id,
                                           CheckCode = Check.CheckCode,
                                           PayeeId = Check.PayeeId,
                                           TotalAmount = Check.TotalAmount,
                                           BankId = Check.BankId,
                                           Bank = Bank,
                                           Notes = Check.Notes,
                                           CompanyId = Check.CompanyId,
                                           CheckProductDetails = Check.CheckProductDetails,
                                           CheckAccountDetails = Check.CheckAccountDetails
                                       }).ToListAsync();

                return checkDtos;
            }
        }

        public async Task<Check> AddCheck(CheckDto input)
        {

            input.CompanyId = _CompanyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;
            if (input.Id > 0)
            {
                await _CheckAccountRepository.DeleteAsync(x => x.CheckId == input.Id);
                await _CheckProductRespository.DeleteAsync(x => x.CheckId == input.Id);
                var Check = await _CheckRepository.FirstOrDefaultAsync(x => x.Id == input.Id);
                if (Check == null) { return new Check(); };
                ObjectMapper.Map(input, Check);
               return await _CheckRepository.UpdateAsync(Check);

            }
            else
            {
                return await _CheckRepository.InsertAsync(ObjectMapper.Map<Check>(input));
            }
        }

        [HttpPost]
        public async Task<CheckDto> GetCheckById(CheckDto input)
        {
            try
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.SoftDelete))
                {

                    var Check = await _CheckRepository.GetAll()
                                            .Where(x => x.Id == input.Id)
                                            .Include(x => x.CheckProductDetails)
                                            .Include(x => x.CheckAccountDetails)
                                            .FirstOrDefaultAsync();
                    return ObjectMapper.Map<CheckDto>(Check);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task DeleteCheck(CheckDto input)
        {
            var Check = await _CheckRepository.FirstOrDefaultAsync(x => x.Id == input.Id);
            if (Check == null) { return; };
            Check.IsDeleted = true;
            await _CheckRepository.UpdateAsync(Check);
        }

        public List<Dictionary<string, object>> GetCheckFooter()
        {
            Type EnumType = typeof(CheckFooterEnum);
            List<Dictionary<string, object>> DisplayNamesAndValues = new List<Dictionary<string, object>>();

            foreach (var Value in Enum.GetValues(EnumType))
            {
                MemberInfo Member = EnumType.GetMember(Value.ToString())[0];
                var DisplayAttribute = Member.GetCustomAttribute<DisplayAttribute>();
                string DisplayName = DisplayAttribute.Name;
                var Item = new Dictionary<string, object>
            {
                { "name", DisplayName },
                { "value", Value }
            };
                DisplayNamesAndValues.Add(Item);
            }

            return DisplayNamesAndValues;
        }

        public async Task<CheckSetupDto> GetCheckSetup()
        {
            var Company = await _CompanyRepository.FirstOrDefaultAsync(x => x.TenantId == (int)AbpSession.TenantId);

            var CheckSetupData = await _CheckSetupRepository.FirstOrDefaultAsync(x => x.CompanyId == Company.Id);

            if (CheckSetupData == null)
            {
                return new CheckSetupDto
                {
                    CompanyId = Company.Id,
                    CheckStyle = "voucher",
                    FirstFooter = 1,
                    SecondFooter = 2,
                    ThirdFooter = 3,
                    BankId = _BankRepository.GetAll().FirstOrDefault().Id,
                    CompanyName = Company.Name,
                };
            }

            return ObjectMapper.Map<CheckSetupDto>(CheckSetupData);
        }

        public async Task SaveCheckSetup(CheckSetupDto input)
        {
            try
            {
                if (input.Id > 0)
                {
                    var CheckSetup = await _CheckSetupRepository.FirstOrDefaultAsync(x => x.Id == input.Id);
                    if (CheckSetup == null) { return; };
                    ObjectMapper.Map(input, CheckSetup);
                    await _CheckSetupRepository.UpdateAsync(CheckSetup);
                }
                else
                {
                    var companyId = _CompanyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Id;
                    input.CompanyId = companyId;
                    await _CheckSetupRepository.InsertAsync(new CheckSetup
                    {
                        CheckStyle = input.CheckStyle,
                        CompanyId = input.CompanyId,
                        CompanyName = input.CompanyName,
                        BankId = input.BankId,
                        AddressLine1 = input.AddressLine1,
                        AddressLine2 = input.AddressLine2,
                        AddressLine3 = input.AddressLine3,
                        FirstFooter = input.FirstFooter,
                        SecondFooter = input.SecondFooter,
                        ThirdFooter = input.ThirdFooter
                    });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<PayeeDto>> GetPayee()
        {
            var Company = await _CompanyRepository.FirstOrDefaultAsync(x => x.TenantId == (int)AbpSession.TenantId);

            var Customers = await _CustomerRepository.GetAllListAsync(x => x.TenantId == (int)AbpSession.TenantId);

            var Vendors = await _VendorRepository.GetAllListAsync(x => x.CompanyId == Company.Id);

            var PayeeList = new List<PayeeDto>();

            PayeeList.AddRange(Customers.Select(x => new PayeeDto
            {
                Id = x.Id  + "-Customer",
                PayeeName = x.Name,
                PayeeType = "Customer"
            }));

            PayeeList.AddRange(Vendors.Select(x => new PayeeDto
            {
                Id = x.Id + "-Vendor",
                PayeeName = x.VendorName,
                PayeeType = "Vendor"
            }));

            return PayeeList;

        }



    }
}
