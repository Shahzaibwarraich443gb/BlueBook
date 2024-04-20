using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using AccountingBlueBook.Entities.Main;
using System.Linq;
using System.Threading.Tasks;
using Abp.ObjectMapping;
using AccountingBlueBook.AppServices.Customers.Dto;
using Microsoft.EntityFrameworkCore;
using AccountingBlueBook.Entities.NormalizeEntities;
using AccountingBlueBook.AppServices.SourceReferralTypes.Dto;
using AccountingBlueBook.AppServices.Phones.Dto;
using AccountingBlueBook.AppServices.SalesPersonTypes.Dto;
using AccountingBlueBook.AppServices.Emails.Dto;
using System.Linq.Dynamic.Core;
using AccountingBlueBook.Entities.MainEntities;
using Abp.Collections.Extensions;
using Abp.Extensions;
using System.Collections.Generic;
using System;
using AccountingBlueBook.AppServices.Spouses.Dto;
using Abp.EntityFrameworkCore.Repositories;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AccountingBlueBook.AppServices.Param;
using Castle.Core.Resource;
using AccountingBlueBook.AppServices.Dependents.Dto;
using Microsoft.AspNetCore.Mvc;
using AccountingBlueBook.AppServices.CustomerTypes.Dto;
using Org.BouncyCastle.Math.EC.Rfc7748;
using Abp.AutoMapper;
using System.Text;
using System.IO;
using MimeKit;
using AccountingBlueBook.Authorization.Users;
using AccountingBlueBook.Authorization.Users.Dto;
using Abp.UI;
using Abp.Authorization;
using AccountingBlueBook.Authorization.Roles;

namespace AccountingBlueBook.AppServices.Customers
{
    public class CustomerAppService : AccountingBlueBookAppServiceBase, ICustomerAppService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<ContactInfo> _contactInfoRepository;
        private readonly IRepository<SourceReferralType> _sourceReferralTypeRepository;
        private readonly IRepository<Spouse, long> _spouseRepository;
        private readonly IRepository<Dependent> _dependentRepository;
        private readonly IRepository<CustomerEmailAddress> _customerEmailRepository;
        private readonly IRepository<CustomerPhoneNumber> _customerPhoneRepository;
        private readonly IRepository<Address> _customerAddressRepository;
        private readonly IRepository<CustomerUser> _customerUserRepository;
        private readonly IRepository<CustomerPassword> _passwordRepository;
        private readonly IRepository<CustomerTaxSelection, long> _CustomerTaxSelectionRepository;
        private readonly IRepository<Company> _CompanyRepository;
        private readonly IRepository<DLState> _DlStateRepository;
        private readonly IEmailAppServices _emailAppService;
        private readonly IPermissionManager _permissionManager;
        private readonly RoleManager _roleManager;
        private IObjectMapper ObjectMapper;

        public CustomerAppService(IRepository<Customer> customerRepository
            , IObjectMapper objectMapper,
            IRepository<CustomerEmailAddress> customerEmailRepository,
            IRepository<CustomerPhoneNumber> customerPhoneRepository,
            IRepository<Address> customerAddressRepository,
            IRepository<CustomerUser> customerUserRepository,
            IRepository<ContactInfo> customerContactInfoRepository,
            IRepository<SourceReferralType> sourceReferralTypeRepository,
            IRepository<Spouse, long> spouseRepository,
            IRepository<Dependent> dependentRepository,
            IRepository<CustomerPassword> passwordRepository,
            IRepository<CustomerTaxSelection, long> customerTaxSelectionRepository,
            IRepository<Company> companyRepository,
            IEmailAppServices emailAppService,
            IPermissionManager permissionManager,
            RoleManager roleManager,
            IRepository<DLState> dlStateRepository)
        {
            _customerRepository = customerRepository;
            ObjectMapper = objectMapper;
            _customerEmailRepository = customerEmailRepository;
            _customerPhoneRepository = customerPhoneRepository;
            _customerAddressRepository = customerAddressRepository;
            _customerUserRepository = customerUserRepository;
            _contactInfoRepository = customerContactInfoRepository;
            _sourceReferralTypeRepository = sourceReferralTypeRepository;
            _spouseRepository = spouseRepository;
            _dependentRepository = dependentRepository;
            _passwordRepository = passwordRepository;
            _CustomerTaxSelectionRepository = customerTaxSelectionRepository;
            _CompanyRepository = companyRepository;
            _DlStateRepository = dlStateRepository;
            _emailAppService = emailAppService;
        }

        public async Task<CreateOrEditCustomerDto> CreateOrEdit(CreateOrEditCustomerDto input)
        {
            if (input.Id > 0)
                return await Update(input);
            else
                return await Create(input);
        }

        public async Task<bool> SaveSpouse(SpouseDto instance)
        {
            if (instance.Id > 0)
            {
                var spouseObj = await _spouseRepository.FirstOrDefaultAsync(instance.Id);
                spouseObj.Name = instance.Name;
                spouseObj.DLIssue = instance.DLIssue;
                spouseObj.SSN = instance.SSN;
                spouseObj.DateOfBirth = instance.DateOfBirth;
                spouseObj.SpouseJobDescription = instance.SpouseJobDescription;
                spouseObj.DrivingLicense = instance.DrivingLicense;
                spouseObj.DLIssue = instance.DLIssue;
                spouseObj.DLExpiry = instance.DLExpiry;
                spouseObj.DLState = instance.DLState;
                spouseObj.Code = instance.Code;
                var data = await _spouseRepository.UpdateAsync(spouseObj);
            }
            else
            {
                var spouseObj = new Spouse();
                spouseObj.Name = instance.Name;
                spouseObj.DLIssue = instance.DLIssue;
                spouseObj.SSN = instance.SSN;
                spouseObj.DateOfBirth = instance.DateOfBirth;
                spouseObj.SpouseJobDescription = instance.SpouseJobDescription;
                spouseObj.DrivingLicense = instance.DrivingLicense;
                spouseObj.DLIssue = instance.DLIssue;
                spouseObj.DLExpiry = instance.DLExpiry;
                spouseObj.DLState = instance.DLState;
                spouseObj.Code = instance.Code;
                var data = await _spouseRepository.InsertAsync(spouseObj);
                var custObj = await _customerRepository.FirstOrDefaultAsync(instance.customerId);
                await CurrentUnitOfWork.SaveChangesAsync();
                if (custObj != null)
                {
                    custObj.SpouseId = data.Id;
                    await _customerRepository.UpdateAsync(custObj);
                }
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return true;
        }


        public async Task<bool> SaveDependent(List<Dependent> instance, int? customerId = 0)
        {
            try
            {
                var DependentList = new List<int>();
                foreach (var data in instance)
                {
                    var dependentObj = new Dependent();
                    dependentObj.Id = data.Id;
                    dependentObj.Name = data.Name;
                    dependentObj.SSN = data.SSN;
                    dependentObj.DateOfBirth = data.DateOfBirth;
                    dependentObj.relation = data.relation;
                    var d = new Dependent();
                    if (data.Id == 0)
                    {
                        d = await _dependentRepository.InsertAsync(dependentObj);
                    }
                    else
                    {
                        d = await _dependentRepository.UpdateAsync(dependentObj);
                    }

                    DependentList.Add(d.Id);

                    await CurrentUnitOfWork.SaveChangesAsync();
                }


                var customer = await _customerRepository.FirstOrDefaultAsync(customerId ?? 0);
                customer.Dependent = instance;
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        [UnitOfWork]
        private async Task<CreateOrEditCustomerDto> Update(CreateOrEditCustomerDto input)
        {
            var customer = await _customerRepository.FirstOrDefaultAsync((int)input.Id);
            customer.BussinessName = input.BussinessName;
            customer.TaxId = input.TaxId;
            customer.BusinessDescription = input.BusinessDescription;
            customer.EFTPS = input.EFTPS;
            customer.Website = input.Website;
            customer.FiscalYearStart = input.FiscalYearStart;
            customer.FiscalYearEnd = input.FiscalYearEnd;
            customer.DateOfBirth = input.DateOfBirth;
            customer.SSN = input.CustomerInfo.SSN;
            customer.CustomerTypeId = input.CustomerInfo.customerTypeId;
            customer.DrivingLicense = input.CustomerInfo.DrivingLicense;
            customer.DLIssue = input.CustomerInfo.DLIssue;
            customer.DLExpiry = input.CustomerInfo.DLExpiry;
            customer.DLState = input.CustomerInfo.DLState;

            customer.Code = input.CustomerInfo.Code;
            customer.Name = input.Name;
            customer.JobDescription = input.JobDescription;
            customer.StateUserName = input.StateUserName;
            customer.StateUserPassword = input.StateUserPassword;
            customer.SpouseId = input.SpouseId;
            customer.LanguageId = input.LanguageId;
            customer.Comment = input.Comment;
            customer.EthnicityId = input.EthnicityId;
            customer.ContactPersonTypeId = input.ContactPersonTypeId;
            customer.SourceReferralTypeId = input.SourceReferralTypeId;
            customer.SalesPersonTypeId = input.SalesPersonTypeId;
            customer.GeneralEntityTypeId = input.GeneralEntityTypeId;
            customer.Spouse = input.spouse;
            customer.NysUsername = input.NysUserName;
            customer.NysPassword = input.NysPassword;
            customer.DetailComment = input.DetailComment;
            customer.EFTPS = input.EFTPS;
            customer.Comment = input.Comment;

            var spouse = new Spouse();
            if (input.CustomerInfo.Spouse.Id > 0)
            {
                spouse = await _spouseRepository.FirstOrDefaultAsync(input.CustomerInfo.Spouse.Id);
            }
            spouse.Name = input.CustomerInfo.Spouse.Name ?? input.CustomerInfo.Spouse.FirstName;
            spouse.SSN = input.CustomerInfo.Spouse.SSN;
            spouse.DateOfBirth = input.CustomerInfo.Spouse.DateOfBirth;
            spouse.SpouseJobDescription = input.CustomerInfo.Spouse.SpouseJobDescription;
            spouse.DLIssue = input.CustomerInfo.Spouse.DLIssue;
            spouse.DrivingLicense = input.CustomerInfo.Spouse.DrivingLicense;
            spouse.DLState = input.CustomerInfo.Spouse.DLState;
            spouse.DLExpiry = input.CustomerInfo.Spouse.DLExpiry;
            spouse.Code = input.CustomerInfo.Spouse.Code;
            if (spouse != null)
            {
                customer.Spouse = spouse;
            }

            customer.Dependent = new List<Dependent>();

            foreach (var dep in input.CustomerInfo.Dependent)
            {

                var dependent = new Dependent();
                if (dep.Id > 0)
                {
                    dependent = await _dependentRepository.FirstOrDefaultAsync(dep.Id ?? 0);

                }
                dependent.Name = dep.Name;
                dependent.SSN = dep.SSN;
                dependent.DateOfBirth = dep.DateOfBirth;
                dependent.relation = dep.Relation;
                dependent.CustomerId = customer.Id;
                if (dep != null)
                {
                    if (dep.Id > 0)
                    {
                        await _dependentRepository.UpdateAsync(dependent);
                    }
                    else
                    {
                        customer.Dependent.Add(dependent);
                    }

                }

            }

            //customer.Addresses = input.Addresses;
            //customer.PhoneNumbers = input.PhoneNumbers;
            //customer.Emials = input.Emials;

            await _customerRepository.UpdateAsync(customer);
            await UpdateRelevantEntities(input);
            return input;

        }

        public async Task<CustomerInfoDto> GetCustomer(ParamDto input)
        {
            try
            {
                var customer = await _customerRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
                int srcRefTypeId = customer.SourceReferralTypeId ?? 0;
                customer.SourceReferralType = await _sourceReferralTypeRepository.FirstOrDefaultAsync(srcRefTypeId);
                ParamDto paramDto = new();
                paramDto.Id = customer.SpouseId != null ? (int)customer.SpouseId : 0;
                var spouse = await GetSpouse(new EntityDto<long>() { Id = (long)paramDto.Id });
                var data = ObjectMapper.Map<CustomerInfoDto>(customer);
                if (spouse != null)
                {
                    data.Spouse = new SpouseDto();
                    data.Spouse.Id = spouse.Id;
                }
                var dependents = await DependentGetListByCustomerId(customer.Id);
                if (dependents != null)
                {
                    data.dependentId = dependents.Select(x => x.Id).ToList();
                }
                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<SpouseDto> GetSpouse(EntityDto<long> input)
        {
            var spouse = await _spouseRepository.FirstOrDefaultAsync(input.Id);
            var data = ObjectMapper.Map<SpouseDto>(spouse);
            return data;
        }

        public async Task<Dependent> DependentGet(int? id)
        {
            var dep = await _dependentRepository.FirstOrDefaultAsync(id ?? 0);
            if (dep == null)
            {
                return new Dependent();
            }
            var data = ObjectMapper.Map<Dependent>(dep);
            return data;
        }


        public async Task<List<Dependent>> DependentGetListByCustomerId(int custId)
        {
            var dep = await _dependentRepository.GetAll().Where(x => x.CustomerId == custId).ToListAsync();
            return dep != null ? dep : new List<Dependent>();
        }


        private async Task UpdateRelevantEntities(CreateOrEditCustomerDto input)
        {
            await _customerAddressRepository.GetAll().Where(x => x.CustomerId == input.Id).ExecuteDeleteAsync();
            await _customerEmailRepository.GetAll().Where(x => x.CustomerId == input.Id).ExecuteDeleteAsync();
            await _customerPhoneRepository.GetAll().Where(x => x.CustomerId == input.Id).ExecuteDeleteAsync();
            await AddCustomerRelevantData(input);
        }

        /// <summary>
        /// Update Customer Details
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task<CreateOrEditCustomerDto> UpdateCustomerDetail(CreateOrEditCustomerDto input)
        {
            var customer = await _customerRepository.FirstOrDefaultAsync((int)input.Id);
            if (input.Detail != null && input.Detail.Language.Id > 0) customer.LanguageId = input.Detail.Language.Id;
            if (input.Detail != null && input.Detail.Ethnicity.Id > 0) customer.EthnicityId = input.Detail.Ethnicity.Id;

            if (input.Detail != null && input.Detail.SalesPersonType.Id > 0) customer.SalesPersonTypeId = input.Detail.SalesPersonType.Id;
            if (input.JobTitleId != null && input.JobTitleId > 0) customer.JobTitleId = input.JobTitleId;
            if (input.NysUserName != null) customer.NysUsername = input.NysUserName;
            if (input.NysPassword != null) customer.NysPassword = input.NysPassword;
            if (input.DetailComment != null) customer.DetailComment = input.DetailComment;
            if (input.EFTPS != null) customer.EFTPS = input.EFTPS;

            await _customerRepository.UpdateAsync(customer);

            return input;
        }

        /// <summary>
        /// Update Customer User
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>new Date(depRes[i].dateOfBirth.format('YYYY-MM-DD'))
        [UnitOfWork]
        public async Task<CreateOrEditCustomerDto> UpdateCustomerUser(CreateOrEditCustomerDto input)
        {
            if (input.UserPassword.Count > 0)
            {
                await _customerUserRepository.GetAll().Where(x => x.CustomerId == input.Id).ExecuteDeleteAsync();
                var customerUser = ObjectMapper.Map<List<CustomerUser>>(input?.UserPassword);
                customerUser.ForEach(x => { x.CustomerId = input.Id; x.Id = 0; });
                //AccountingBlueBookDbContext context = CurrentUnitOfWork.GetDbContext<AccountingBlueBookDbContext>();
                //await context.CustomerUsers.AddRangeAsync(customerUser);
                foreach (var item in customerUser)
                {
                    await _customerUserRepository.InsertAsync(item);
                }
            }
            return input;
        }

        /// <summary>
        /// Update Customer address
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task<CreateOrEditCustomerDto> UpdateCustomerAddress(CreateOrEditCustomerDto input)
        {
            if (input?.Address.Count > 0)
            {
                await _customerAddressRepository.GetAll().Where(x => x.CustomerId == input.Id).ExecuteDeleteAsync();
                List<Address> addressList = ObjectMapper.Map<List<Address>>(input?.Address);
                foreach (var data in addressList)
                {
                    data.CustomerId = input.Id;
                }
                await _customerAddressRepository.InsertRangeAsync(addressList);
                await CurrentUnitOfWork.SaveChangesAsync();


                //await _contactInfoRepository.GetAll().Where(x => x.CustomerId == input.Id).ExecuteDeleteAsync();
                //var address = ObjectMapper.Map<List<Address>>(input?.Address);
                //address.ForEach(x => { x.CustomerId = input.Id; x.Id = 0; });
                // to do: shift this method in efcore project
                //AccountingBlueBookDbContext context = CurrentUnitOfWork.GetDbContext<AccountingBlueBookDbContext>();
                //await context.Addresses.AddRangeAsync(address);
            }
            return input;
        }

        /// <summary>
        /// Update Customer Contact Info
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task<CreateOrEditCustomerDto> UpdateCustomerContactInfo(CreateOrEditCustomerDto input)
        {
            if (input?.ContactPersons.Count > 0)
            {
                await _contactInfoRepository.GetAll().Where(x => x.CustomerId == input.Id).ExecuteDeleteAsync();
                List<ContactInfo> contactPerson = ObjectMapper.Map<List<ContactInfo>>(input?.ContactPersons);
                foreach (var data in contactPerson)
                {
                    data.CustomerId = input.Id;
                }
                await _contactInfoRepository.InsertRangeAsync(contactPerson);
                await CurrentUnitOfWork.SaveChangesAsync();
                // to do: shift this method in efcore project
                //contactPerson.ForEach(x => { x.CustomerId = input.Id; x.Id = 0; });
                //AccountingBlueBookDbContext context = CurrentUnitOfWork.GetDbContext<AccountingBlueBookDbContext>();
                //await context.ContactInfos.AddRangeAsync(contactPerson);
            }
            return input;
        }

        public async Task SaveCustomerTaxSelection(CustomerTaxSelection input)
        {
            var customerTaxSelection = await _CustomerTaxSelectionRepository.FirstOrDefaultAsync(x => x.CustomerId == input.CustomerId && (int)x.TaxService == (int)input.TaxService);
            if (customerTaxSelection == null)
            {
                await _CustomerTaxSelectionRepository.InsertAsync(input);
            }
        }

        public async Task<IEnumerable<CustomerTaxSelection>> CustomerTaxSelectionGet(EntityDto input)
        {
            var customerTaxSelection = await _CustomerTaxSelectionRepository.GetAll().Where(x => x.CustomerId == input.Id).ToListAsync();
            return customerTaxSelection;
        }

        [UnitOfWork]
        private async Task<CreateOrEditCustomerDto> Create(CreateOrEditCustomerDto input)
        {
            try
            {
                var customer = new Customer();
                bool custFound = false;
                customer = await _customerRepository.FirstOrDefaultAsync(x => x.Id == input.CustomerInfo.Id);
                if (customer == null)
                {
                    customer = new Customer();
                }
                else
                {
                    custFound = true;
                }
                customer.BussinessName = input.CustomerInfo.BussinessName;
                customer.TaxId = input.CustomerInfo.TaxId;
                customer.FiscalYearEnd = input.CustomerInfo.FiscalYearEnd;
                customer.Name = input.CustomerInfo.Name;
                customer.SSN = input.CustomerInfo.SSN;
                customer.DateOfBirth = input.CustomerInfo?.DateOfBirth;
                customer.JobDescription = input.CustomerInfo?.JobDescription;
                customer.DrivingLicense = input.CustomerInfo?.DrivingLicense;
                customer.DLState = input.CustomerInfo?.DLState;

                customer.DLExpiry = input.CustomerInfo?.DLExpiry;
                customer.Code = input.CustomerInfo.Code;
                customer.DLIssue = input.CustomerInfo.DLIssue;
                customer.Comment = input.Comment;


                //adding customer type due to conflict in model
                if (input.CustomerInfo.CustomerType.Name != null)
                {
                    CustomerType customerType = new();
                    int customerTypeId = input.CustomerInfo.CustomerType.Id ?? 0;
                    customerType.Name = input.CustomerInfo.CustomerType.Name;
                    customer.CustomerType = customerType;
                    if (custFound == true)
                    {
                        customer.CustomerType.Id = customerTypeId;
                    }
                    customer.CustomerTypeId = customerTypeId;
                }

                //adding source referral type due to conflict in model

                //customer.SourceReferralType = ObjectMapper.Map<SourceReferralType>(input.CustomerInfo.SourceReferralType);
                customer.SourceReferralTypeId = input.CustomerInfo.SourceReferralType.Id;

                if (input.CustomerInfo.SourceReferralType.Id > 0)
                    customer.SourceReferralTypeId = input.CustomerInfo.SourceReferralType.Id;
                if (input.CustomerInfo.CustomerType.Id > 0)
                    customer.CustomerTypeId = input.CustomerInfo.CustomerType.Id;
                var spouse = new Spouse();
                if (input.CustomerInfo.Spouse.Id > 0)
                {
                    spouse = await _spouseRepository.FirstOrDefaultAsync(input.CustomerInfo.Spouse.Id);
                }
                spouse.Name = input.CustomerInfo.Spouse.Name ?? input.CustomerInfo.Spouse.FirstName;
                spouse.SSN = input.CustomerInfo.Spouse.SSN;
                spouse.DateOfBirth = input.CustomerInfo.Spouse.DateOfBirth;
                spouse.SpouseJobDescription = input.CustomerInfo.Spouse.SpouseJobDescription;
                spouse.DLIssue = input.CustomerInfo.Spouse.DLIssue;
                spouse.DrivingLicense = input.CustomerInfo.Spouse.DrivingLicense;
                spouse.DLState = input.CustomerInfo.Spouse.DLState;
                spouse.DLExpiry = input.CustomerInfo.Spouse.DLExpiry;
                spouse.Code = input.CustomerInfo.Spouse.Code;
                if (spouse != null)
                {
                    if (spouse.Id > 0)
                    {
                        await _spouseRepository.UpdateAsync(spouse);
                    }
                    else
                    {
                        customer.Spouse = spouse;
                    }
                }

                customer.Dependent = new List<Dependent>();
                List<int> dependentIds = new();
                foreach (var dep in input.CustomerInfo.Dependent)
                {

                    var dependent = new Dependent();
                    if (dep.Id > 0)
                    {
                        dependent = await _dependentRepository.FirstOrDefaultAsync(dep.Id ?? 0);
                    }
                    dependent.Name = dep.Name;
                    dependent.SSN = dep.SSN;
                    dependent.DateOfBirth = dep.DateOfBirth;
                    dependent.relation = dep.Relation;
                    dependent.CustomerId = customer.Id;
                    if (dep != null)
                    {
                        if (dep.Id > 0)
                        {
                            await _dependentRepository.UpdateAsync(dependent);
                        }
                        else
                        {
                            customer.Dependent.Add(dependent);
                        }

                    }

                }

                customer.DependentIds = string.Join(",", dependentIds);


                customer.EFTPS = input.EFTPS;
                customer.NysUsername = input.NysUserName;
                customer.NysPassword = input.NysPassword;
                customer.DetailComment = input.DetailComment;
                customer.JobTitleId = input.JobTitleId;

                //if (input.CustomerInfo.Spouse != null)
                //    customer.Spouse = ObjectMapper.Map<Spouse>(input.CustomerInfo.Spouse);
                //if (input.CustomerInfo.Dependent != null)
                //    customer.Dependent = ObjectMapper.Map<Dependent>(input.CustomerInfo.Dependent);
                input.Id = await _customerRepository.InsertOrUpdateAndGetIdAsync(customer);


                return input;
            }

            catch (Exception ex)
            {
                throw;
            }

        }

        private async Task AddCustomerRelevantData(CreateOrEditCustomerDto input)
        {
            // to do: shift this method in efcore project
            //AccountingBlueBookDbContext context = CurrentUnitOfWork.GetDbContext<AccountingBlueBookDbContext>();

            //await AddCustomerAddresses(input, context);

            //await AddCustomerPhoneNumbers(input, context);

            //await AddCustomerEmails(input, context);
        }

        // to do: shift this method in efcore project
        //private async Task AddCustomerAddresses(CreateOrEditCustomerDto input, AccountingBlueBookDbContext context)
        //{
        //    var addresses = ObjectMapper.Map<List<Address>>(input.Addresses);
        //    addresses.ForEach(x => x.Id = 0);
        //    List<CustomerAddress> customerAddresses = new List<CustomerAddress>();
        //    addresses.ForEach(address => customerAddresses.Add(new CustomerAddress { Address = address, CustomerId = input.Id }));
        //    await context.CustomerAddress.AddRangeAsync(customerAddresses);
        //}

        // to do: shift this method in efcore project
        //private async Task AddCustomerPhoneNumbers(CreateOrEditCustomerDto input, AccountingBlueBookDbContext context)
        //{
        //    var phones = ObjectMapper.Map<List<Phone>>(input.PhoneNumbers);
        //    phones.ForEach(x => x.Id = 0);
        //    List<CustomerPhoneNumber> customerPhoneNumbers = new List<CustomerPhoneNumber>();
        //    phones.ForEach(phone => customerPhoneNumbers.Add(new CustomerPhoneNumber { Phone = phone, CustomerId = input.Id }));
        //    await context.CustomerPhoneNumber.AddRangeAsync(customerPhoneNumbers);
        //}

        // to do: shift this method in efcore project
        //private async Task AddCustomerEmails(CreateOrEditCustomerDto input, AccountingBlueBookDbContext context)
        //{
        //    var emails = ObjectMapper.Map<List<Email>>(input.Emails);
        //    emails.ForEach(x => x.Id = 0);
        //    List<CustomerEmailAddress> customerEmailAddress = new List<CustomerEmailAddress>();
        //    emails.ForEach(email => customerEmailAddress.Add(new CustomerEmailAddress { Email = email, CustomerId = input.Id }));
        //    await context.CustomerEmailAddress.AddRangeAsync(customerEmailAddress);
        //}

        public async Task Delete(EntityDto input)
        {
            var customer = await _customerRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            await _customerRepository.DeleteAsync(customer);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<CustomerDto> Get(EntityDto input)
        {
            var customer = await _customerRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            var data = ObjectMapper.Map<CustomerDto>(customer);
            return data;
        }

        public async Task<CreateOrEditCustomerDto> GetCustomerForEdit(EntityDto input)
        {
            var customer = await _customerRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            var data = ObjectMapper.Map<CreateOrEditCustomerDto>(customer);
            return data;
        }

        private void GetCustomerEmails(EntityDto input, CreateOrEditCustomerDto data)
        {
            var customerEmails = _customerEmailRepository.GetAll().Where(x => x.CustomerId == input.Id).Include(x => x.Email);
            if (customerEmails.Any() && customerEmails.Select(x => x.Email).Any())
            {
                var emails = customerEmails.Select(x => x.Email);
                data.Emails = ObjectMapper.Map<List<CreateOrEditEmailInputDto>>(emails);
            }
        }

        private void GetCustomerPhoneNumbers(EntityDto input, CreateOrEditCustomerDto data)
        {
            var customerPhoneNumbers = _customerPhoneRepository.GetAll().Where(x => x.CustomerId == input.Id).Include(x => x.Phone);
            if (customerPhoneNumbers.Any() && customerPhoneNumbers.Select(x => x.Phone).Any())
            {
                var phoneNumbers = customerPhoneNumbers.Select(x => x.Phone);
                data.PhoneNumbers = ObjectMapper.Map<List<CreateOrEditPhoneDto>>(phoneNumbers);
            }
        }


        public async Task<Customer> GetCustomerLicenseData(int customerId)
        {
            try
            {
                var customer = await _customerRepository.FirstOrDefaultAsync(customerId);
                return customer;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<string> GetCustomerComment(int customerId)
        {
            try
            {
                var customer = await _customerRepository.FirstOrDefaultAsync(customerId);
                if (customer != null)
                {
                    return customer.Comment == null ? "" : customer.Comment;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> SaveCustomerComment(int customerId, string comment = "")
        {
            try
            {
                if (comment != null)
                {
                    var customer = await _customerRepository.FirstOrDefaultAsync(customerId);

                    if (customer == null)
                    {
                        return true;
                    }

                    customer.Comment = comment;

                    await _customerRepository.UpdateAsync(customer);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Customer> GetCRMData(int customerId)
        {
            try
            {
                var customer = await _customerRepository.FirstOrDefaultAsync(customerId);
                return customer;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Customer> GetCustomerTodoListData(int customerId)
        {
            try
            {
                var customer = await _customerRepository.FirstOrDefaultAsync(customerId);
                return customer;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> SaveCustomerPassword(CustomerPassword instance)
        {
            try
            {

                if (instance.Id == 0)
                {

                    await _passwordRepository.InsertAsync(instance);
                }
                else
                {
                    await _passwordRepository.UpdateAsync(instance);
                }

                await CurrentUnitOfWork.SaveChangesAsync();


                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteCustomerPassword(int id)
        {
            await _passwordRepository.DeleteAsync(id);
            await CurrentUnitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddPasswordComment(string comment, int customerId)
        {

            var customer = await _customerRepository.FirstOrDefaultAsync(customerId);
            customer.passwordComment = comment;

            await CurrentUnitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<string> PasswordCommentGet(int customerId)
        {

            var customer = await _customerRepository.FirstOrDefaultAsync(customerId);
            return customer.passwordComment;
        }

        public async Task<List<CustomerPassword>> CustomerPasswordGet(int customerId)
        {
            try
            {
                return await _passwordRepository.GetAll().Where(x => x.CustomerId == customerId).ToListAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<PagedResultDto<CustomerDto>> GetAll(PagedResultInputRequestDto input)
        {
            try
            {

                var dlStates = _DlStateRepository.GetAll().AsEnumerable();

                var filteredQuery = _customerRepository.GetAll()
                    .Include(x => x.ContactPersonType)
                    .Include(x => x.Address)
                    .Include(x => x.ContactInfo)
                    .Include(x => x.Company)
                    .WhereIf(!input.Keyword.IsNullOrEmpty(), e => e.ContactInfo.Any() && e.ContactInfo.Any(c => c.Email.ToLower().Contains(input.Keyword.ToLower())) ||
                                e.ContactInfo.Any() && e.ContactInfo.Any(c => c.Number.ToLower().Contains(input.Keyword.ToLower())) ||
                                e.Address.Any() && e.Address.Any(c => c.CompleteAddress.ToLower().Contains(input.Keyword.ToLower())) ||
                                e.Name.ToLower().Contains(input.Keyword.ToLower()) ||
                                e.Id == int.Parse(input.Keyword)
                                ).AsQueryable();

                if (!filteredQuery.Any())
                {
                    return null;
                }

                var CompanyName = _CompanyRepository.FirstOrDefault(x => x.TenantId == (int)AbpSession.TenantId).Name;


                var data = from o in filteredQuery
                           select new CustomerDto
                           {
                               Id = o.Id,
                               Name = o.Name,
                               BussinessName = o.BussinessName,
                               FiscalYearEnd = o.FiscalYearEnd,
                               BusinessDescription = o.BusinessDescription,
                               DateOfBirth = o.DateOfBirth,
                               JobDescription = o.JobDescription,
                               StateName = dlStates.FirstOrDefault(x => x.Id == o.DLState).StateName,
                               StateCode = dlStates.FirstOrDefault(x => x.Id == o.DLState).StateCode,
                               SocialSecurityNumber = o.SSN,
                               PhoneNumber = o.ContactInfo.FirstOrDefault().Number,
                               Email = o.ContactInfo.FirstOrDefault().Email,
                               Address = o.Address.FirstOrDefault().CompleteAddress,
                               RefCopmayId = o.CompanyId,
                               CustomerType = ObjectMapper.Map<CustomerTypeDto>(o.CustomerType),
                               CompanyName = CompanyName,
                               Spouse = o.Spouse,
                               DependentNames = String.Join(',', o.Dependent.Select(x => x.Name).ToList()),
                               DependentRelations = String.Join(',', o.Dependent.Select(x => x.relation).ToList()),

                           };

                return new PagedResultDto<CustomerDto>
                {
                    Items = data.ToList(),
                };
            }
            catch
            {
                return new PagedResultDto<CustomerDto> { };
            }
        }

        /// <summary>
        /// Update Customer Contact Info
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task<CreateOrEditCustomerDto> GetCustomerInfo(int id)
        {
            var customer = await _customerRepository.GetAll()
                .Include(c => c.CustomerType)
                .Include(c => c.SourceReferralType)
                .Include(c => c.Dependent)
                .Include(c => c.Spouse)
                .FirstOrDefaultAsync(x => x.Id == id);

            List<DependentDto> dependents = new();

            if (customer != null)
            {
                foreach (var data in customer.Dependent)
                {
                    var dep = new Dependents.Dto.DependentDto
                    {
                        Id = data.Id,
                        Name = data?.Name,
                        Relation = data?.relation,
                        SSN = data?.SSN,
                        DateOfBirth = data?.DateOfBirth,
                    };
                    dependents.Add(dep);
                }
            }

            CreateOrEditCustomerDto createOrEditCustomerDto = new CreateOrEditCustomerDto()
            {
                Comment = customer.Comment,
                CustomerInfo = new CustomerInfoDto()
                {
                    Id = customer.Id,
                    BussinessName = customer?.BussinessName,
                    TexId = customer?.TexId,
                    FiscalYearEnd = customer?.FiscalYearEnd,
                    Name = customer?.Name,
                    BusinessDescription = customer?.BusinessDescription,
                    TaxId = customer?.TaxId,
                    SocialSecurityNumber = customer?.SocialSecurityNumber,
                    FirstName = customer?.FirstName,
                    MiddleName = customer?.MiddleName,
                    LastName = customer?.LastName,
                    CustomerType = customer?.CustomerType != null ? new CustomerTypes.Dto.CustomerTypeDto
                    {
                        Id = customer.CustomerTypeId.Value,
                        Name = customer?.CustomerType.Name,
                    } : null,
                    SourceReferralType = customer.SourceReferralType != null ? new SourceReferralTypeDto
                    {
                        Id = customer.SourceReferralType.Id,
                        Name = customer?.SourceReferralType.Name,
                    } : null,
                    SSN = customer?.SSN,
                    DateOfBirth = customer?.DateOfBirth,
                    JobDescription = customer?.JobDescription,
                    DrivingLicense = customer?.DrivingLicense,
                    DLIssue = customer?.DLIssue,
                    DLExpiry = customer?.DLExpiry,
                    DLState = customer?.DLState,

                    Code = customer?.Code ?? 0,
                    Spouse = customer.Spouse != null ? new SpouseDto
                    {
                        Id = customer.Spouse.Id,
                        SpouseSuffix = customer?.Spouse?.SpouseSuffix,
                        FirstName = customer?.Spouse?.FirstName,
                        LastName = customer?.Spouse?.LastName,
                        Name = customer?.Spouse?.Name,
                        Email = customer?.Spouse?.Email,
                        SSN = customer?.Spouse?.SSN,
                        SpouseJobDescription = customer?.Spouse?.SpouseJobDescription,
                        DrivingLicense = customer?.Spouse?.DrivingLicense,
                        Code = customer?.Spouse?.Code ?? 0,
                        DLIssue = customer?.Spouse?.DLIssue,
                        DLExpiry = customer?.Spouse?.DLExpiry,
                        DLState = customer?.Spouse?.DLState,
                        DateOfBirth = customer?.Spouse?.DateOfBirth,
                    } : null,


                    Dependent = dependents
                }
            };
            return createOrEditCustomerDto;
        }
        /// <summary>
        /// Get Customer Details
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task<CreateOrEditCustomerDto> GetCustomerDetails(int customerId)
        {
            var customer = await _customerRepository.GetAll()
                .Include(x => x.Ethnicity)
                .Include(x => x.Language)
                .Include(x => x.SourceReferralType)
                .Include(x => x.SalesPersonType)
                .FirstOrDefaultAsync(x => x.Id == customerId);

            return new CreateOrEditCustomerDto()
            {
                Detail = new DetailDto()
                {
                    Ethnicity = customer.Ethnicity != null ? new Ethnicities.Dto.EthnicityDto()
                    {
                        Id = customer.Ethnicity.Id,
                        Name = customer.Name,
                    } : null,
                    Language = customer.Language != null ? new Languages.Dto.LanguageDto()
                    {
                        Id = customer.Language.Id,
                        Name = customer.Language.Name,
                    } : null,

                    SalesPersonType = customer.SalesPersonType != null ? new SalesPersonTypeDto()
                    {
                        Id = customer.SalesPersonType.Id,
                        Name = customer.SalesPersonType.Name,
                    } : null
                },
                EFTPS = customer.EFTPS,
                NysUserName = customer.NysUsername,
                NysPassword = customer.NysPassword,
                DetailComment = customer.DetailComment,
                JobTitleId = customer.JobTitleId,
                Id = customer.Id,

            };
        }

        public async Task<List<Customer>> GetCustomersByTenantId()
        {
            try
            {
                return await _customerRepository.GetAllListAsync(x => x.TenantId == (int)AbpSession.TenantId);


            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// Get Customer contact
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task<List<ContactInfoDto>> GetCustomerContact(int customerId)
        {
            var contactInfo = await _contactInfoRepository.GetAll()
                .Where(x => x.CustomerId == customerId)
                .ToListAsync();
            var mapdata = ObjectMapper.Map<List<ContactInfoDto>>(contactInfo);
            return mapdata;
        }

        /// <summary>
        /// Get Customer Address
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task<List<CustomerAddressDto>> GetCustomerAddress(int customerId)
        {
            var data = await _customerAddressRepository.GetAll()
                 .Where(x => x.CustomerId == customerId)
                .ToListAsync();
            var mapdata = ObjectMapper.Map<List<CustomerAddressDto>>(data);
            return mapdata;
        }

        /// <summary>
        /// Get Customer Users
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task<List<UserNamePasswordDto>> GetCustomerUsers(int customerId)
        {
            var data = await _customerUserRepository.GetAll()
                 .Where(x => x.CustomerId == customerId)
                .ToListAsync();
            var mapdata = ObjectMapper.Map<List<UserNamePasswordDto>>(data);
            return mapdata;
        }

        public async Task SendEmailToCustomer(string Subject, string EmailBody, string FileName, int CustomerId)
        {
            var TenantId = AbpSession.TenantId;
            string subject = Subject;
            StringBuilder emailbody = new StringBuilder();
            emailbody.Append(EmailBody);

            var folderName = Path.Combine("wwwroot", "Files", "CustomerAttachments");
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = emailbody.ToString();

            var attachmentStreams = new List<Tuple<string, MemoryStream>>(); // Move this outside the loop

            if (Directory.Exists(folderPath))
            {
                var fileNames = FileName.Split(',');
                foreach (var fileName in fileNames)
                {
                    var filePath = Path.Combine(folderPath, fileName);
                    if (File.Exists(filePath))
                    {
                        using (var stream = new FileStream(filePath, FileMode.Open))
                        {
                            var memoryStream = new MemoryStream();
                            await stream.CopyToAsync(memoryStream);

                            memoryStream.Position = 0;
                            bodyBuilder.Attachments.Add(fileName, memoryStream);
                            attachmentStreams.Add(new Tuple<string, MemoryStream>(fileName, memoryStream));
                        }
                    }
                }
            }
            
            string sendEmail = _contactInfoRepository.FirstOrDefault(a => a.CustomerId == CustomerId).Email;
            if (sendEmail == null)
            {
                throw new UserFriendlyException("Please Add Email Address ", "Something Wrong!!");
            }

            await _emailAppService.SendMail(new EmailsDto
            {
                Subject = subject,
                Body = emailbody.ToString(),
                ToEmail = sendEmail,
                Streams = attachmentStreams
            });
        }

    }
}