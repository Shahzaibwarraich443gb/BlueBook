﻿using Abp.Application.Features;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.Zero.EntityFrameworkCore;
using AccountingBlueBook.Authorization.Roles;
using AccountingBlueBook.Authorization.Users;
using AccountingBlueBook.Editions;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;

using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace AccountingBlueBook.MultiTenancy
{
    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        public IAbpSession AbpSession { get; set; }

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;

        // private readonly IRepository<TenantSetting> _tenantSettingRepository;

        private readonly IUserEmailer _userEmailer;
        private readonly UserManager _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Employee, long> _employeeRepository;
        
        //  private readonly IRepository<SubscribableEdition> _subscribableEditionRepository;
        private readonly RoleManager _roleManager;
        private readonly EditionManager _editionManager;
        private readonly IPermissionManager _permissionManager;
        public TenantManager(
            IUserEmailer userEmailer,
            IRepository<Tenant> tenantRepository,
            EditionManager editionManager,
            IRepository<Company> companyRepository,
             IRepository<Employee,long>employeRepository,
        // IRepository<SubscribableEdition> subscribableEditionRepository,
        IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
          //  IRepository<TenantSetting> tenantSettingRepository,
          
            IUnitOfWorkManager unitOfWorkManager,
            IAbpZeroDbMigrator abpZeroDbMigrator,
            UserManager userManager,
        RoleManager roleManager,
           
        IPasswordHasher<User> passwordHasher,
            IAbpZeroFeatureValueStore featureValueStore,
          
            IPermissionManager permissionManager) 
            : base(
                tenantRepository, 
                tenantFeatureRepository, 
                editionManager,
                featureValueStore)
        {
            AbpSession = NullAbpSession.Instance;

           // _subscribableEditionRepository = subscribableEditionRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _abpZeroDbMigrator = abpZeroDbMigrator;
            _roleManager = roleManager;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _userEmailer = userEmailer;
          //  _tenantSettingRepository = tenantSettingRepository;
          
            _permissionManager = permissionManager;
            _editionManager = editionManager;
            _companyRepository = companyRepository;
            _employeeRepository = employeRepository;
        }
        public override IQueryable<Tenant> Tenants => base.Tenants;

        public async Task<int> CreateWithAdminUserAsync(
            
            string tenancyName,
             string phoneNumber, 
             bool isTwoFactorEnabled,
            string name,
            string adminPassword,
            string adminEmailAddress,
            string connectionString,
            bool isActive,
            int? editionId,
         
        
        bool shouldChangePasswordOnNextLogin,
            bool sendActivationEmail,
            DateTime? subscriptionEndDate,
            bool isInTrialPeriod,
            int users,
            int space,
            string emailActivationLink = "",
            string editionName = "",
            string city="",
            DateTime FascialStartDate = default,
            DateTime FascialEndDate = default,
            string state="",
            string  country="",
            string companyaddress="",
            string zipcode="",
            string FirstName="",
            string LastName="",
            string Suffix=""
            
          //  string EmailAddress=""
            

           
              
               )
        {
            int newTenantId;
            long newAdminId;

            //await CheckEditionAsync(editionId, isInTrialPeriod);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                //Create tenant
                var tenant = new Tenant(tenancyName, name)
                {
                    IsActive =isActive,
                    EditionId = editionId,
                    SubscriptionEndDateUtc = subscriptionEndDate?.ToUniversalTime(),
                    IsInTrialPeriod = isInTrialPeriod,
                    ConnectionString = connectionString.IsNullOrWhiteSpace() ? null : SimpleStringCipher.Instance.Encrypt(connectionString),
                   TenantKey = Guid.NewGuid(),
                  
                };

                await CreateAsync(tenant);
                await _unitOfWorkManager.Current.SaveChangesAsync(); //To get new tenant's id.

                //Create tenant database
                _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

                //We are working entities of new tenant, so changing tenant filter
                using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                {
                    //Create static roles for new tenant
                    CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get static role ids

                    //grant all permissions to admin role
                    var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);

                    
                    // if (!editionName.IsNullOrWhiteSpace() && (editionName == "Trial" || editionName == "Premium"))
                    //     await _roleManager.GrantAllPermissionsAsync(adminRole);
                    // else
                    //{
                    //var grantedPermissions = await GetPermissions((int)editionId);
                    //   await _roleManager.SetGrantedPermissionsAsync(adminRole.Id, grantedPermissions);
                    // }
                    await _roleManager.GrantAllPermissionsAsync(adminRole);

                    //insert company

                    var CreatedCompnay = new Company()
                    {
                        TenantId = tenant.Id,
                        Name = name,
                        City = city,
                        State = state,
                        FiscalYearStart = FascialStartDate,
                        FiscalYearEnd= FascialEndDate,
                        Country =country,
                        CompanyAddress=companyaddress,
                        ZipCode=zipcode,
                        IsActive=true
                     
                    };
                    var companyId = _companyRepository.InsertAndGetId(CreatedCompnay);

                    var employee = new Employee()
                    {
                        //employee=companyId,
                        //employee.tenaId=tenant.Id,
                        TenantId=tenant.Id,
                        
                        FirstName = FirstName,
                        LastName = LastName,
                        Suffix = Suffix,
                        Email = adminEmailAddress,
                        PhoneNumber = (int)Convert.ToInt64(phoneNumber),
                        City = city,
                        State = state,
                        Country = country,
                        ZipCode = zipcode,
                        IsActive = true

                    };

                    var employeeId = _employeeRepository.InsertAndGetId(employee);


                    //Create admin user for the tenant
                    var userName = adminEmailAddress.Substring(0, adminEmailAddress.IndexOf("@"));
                    var adminUser = User.CreateTenantAdminUser(tenant.Id, adminEmailAddress, $"{User.AdminUserName}_{Clock.Now.Ticks}_{userName}", tenancyName);

                    adminUser.IsActive = true;
                    adminUser.CompanyId = companyId;
                    adminUser.EmployeeId = employeeId;

                    adminUser.PhoneNumber = phoneNumber;
                    adminUser.IsTwoFactorEnabled = isTwoFactorEnabled;
                    adminUser.LastOnlineTime = DateTime.Now;

                    if (adminPassword.IsNullOrEmpty())
                    {
                        adminPassword = "123qwe";
                    }
                    else
                    {
                        await _userManager.InitializeOptionsAsync(AbpSession.TenantId);
                        foreach (var validator in _userManager.PasswordValidators)
                        {
                            CheckErrors(await validator.ValidateAsync(_userManager, adminUser, adminPassword));
                        }

                    }
                    
                    adminUser.Password = _passwordHasher.HashPassword(adminUser, adminPassword);
                   // await _userManager.CreateAsync(adminUser);
                    
                    CheckErrors(await _userManager.CreateAsync(adminUser));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get admin user's id

                    //Assign admin user to admin role!
                    CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));
                
                    
                    //#region background job
                    //var tenantSettings = new TenantSetting()
                    //{
                    //    IsActive = true,
                    //    ScreenshotInterval = 5,
                    //    ProductivityBenchMark = 70,
                    //    DefaultLanguage = "en",
                    //    TenantId = tenant.Id,
                    //    CreatorUserId = adminUser.Id
                    //};

                    //await _tenantSettingRepository.InsertAsync(tenantSettings);

                    //#endregion

                    //Send activation email
                    if (sendActivationEmail)
                    {
                        adminUser.SetNewEmailConfirmationCode();
                        await _userEmailer.WelcomeEmail(adminUser, emailActivationLink, adminPassword);
                    }

                    await _unitOfWorkManager.Current.SaveChangesAsync();

                    newTenantId = tenant.Id;
                    newAdminId = adminUser.Id;
                }

                await uow.CompleteAsync();
            }

         
            //Used a second UOW since UOW above sets some permissions and _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync needs these permissions to be saved.
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(newTenantId))
                {
                    // _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(new UserIdentifier(newTenantId, newAdminId));
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    await uow.CompleteAsync();
                }
            }

            return newTenantId;
        }
    
        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
