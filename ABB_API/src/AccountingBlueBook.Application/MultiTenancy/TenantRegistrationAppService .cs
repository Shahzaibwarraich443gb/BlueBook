using System;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Abp.Timing;
using Abp.UI;
using Abp.Zero.Configuration;
using AccountingBlueBook.Authorization.Roles;
using AccountingBlueBook.Authorization.Users;
using AccountingBlueBook.Configuration;
using AccountingBlueBook.Editions;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.MultiTenancy.Dto;
using AccountingBlueBook.TwilioConfiguratrions;
using AccountingBlueBook.URL;
using Microsoft.EntityFrameworkCore;

namespace AccountingBlueBook.MultiTenancy
{
    // [AbpAuthorize(PermissionNames.Pages_Tenants)]
    public class TenantRegistationAppService : AccountingBlueBookAppServiceBase, ITenantRegistrationAppService
    {
        private readonly TenantManager _tenantManager;
        public IAppUrlService AppUrlService { get; set; }
        private readonly EditionManager _editionManager;
        private readonly UserManager _userManager;
        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly ITwilioConfiguration _twilioConfiguration;
        private readonly RoleManager _roleManager;
        private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Company> _companyRepository;

        public TenantRegistationAppService(
            IRepository<Tenant, int> repository,
               IRepository<User, long> userRepository,
               IRepository<Company> companyRepository,
        TenantManager tenantManager,
          ITwilioConfiguration twilioConfiguration,
            EditionManager editionManager,
            IMultiTenancyConfig multiTenancyConfig,
            UserManager userManager,
            RoleManager roleManager,
            IAbpZeroDbMigrator abpZeroDbMigrator)
            : base()
        {
            _tenantManager = tenantManager;
            _multiTenancyConfig = multiTenancyConfig;
            _editionManager = editionManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _twilioConfiguration = twilioConfiguration;
            _userRepository = userRepository;
            _companyRepository = companyRepository;
            _abpZeroDbMigrator = abpZeroDbMigrator;
            AppUrlService = NullAppUrlService.Instance;
            
        }

        public async Task<RegisterTenantoutputDto> RegisterTenant(RegisterTenantInputDto input)
         {
            var user1 = await _userRepository.GetAll()
                                      .IgnoreQueryFilters()
                                      .FirstOrDefaultAsync(a => a.EmailAddress == input.AdminEmailAddress && !a.IsDeleted);
            if (user1 != null)
                throw new UserFriendlyException("Email address already exist");

            //char[] deligators = { '@' };
           // string[] user = input.AdminEmailAddress.Split(deligators);
            //if (user[0].Contains("."))
           // {
           //   char[] withDotdeligators = { '@', '.' };
               // string replaceDot = user[0].Replace('.', '_');
               // input.TenancyName = replaceDot;
               // input.Name = replaceDot;
           // }
          //  else
          //  {
                //input.TenancyName = input.AdminEmailAddress.Split(deligators)[0] + "_" + Guid.NewGuid();
               // input.Name = input.AdminEmailAddress.Split(deligators)[0];
                input.TenancyName = input.TenancyName;
                input.Name = input.TenancyName;

           // }

            if (input.EditionId.HasValue)
            {
                await CheckEditionSubscriptionAsync(input.EditionId.Value, input.SubscriptionStartType);
            }
            else
            {
                await CheckRegistrationWithoutEdition();
            }
            //To avoid duplicate email during Tenant registration
            var userwithEmail = await GetUserByChecking(input.AdminEmailAddress);
            if (userwithEmail != null)
            {
                throw new UserFriendlyException(L("Email is already registered \n use another one."));
            }
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                CheckTenantRegistrationIsEnabled();

                //Getting host-specific settings
                var isActive = await IsNewRegisteredTenantActiveByDefault(input.SubscriptionStartType);
                var isEmailConfirmationRequired = Convert.ToBoolean(await SettingManager.GetSettingValueForApplicationAsync(
                    AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin
                ));
            

                DateTime? subscriptionEndDate = null;
                var isInTrialPeriod = false;
                var editionName = string.Empty;
                if (input.EditionId.HasValue)
                {
                    isInTrialPeriod = input.SubscriptionStartType == SubscriptionStartType.Free;

                    //when free trial is selected
                    if (isInTrialPeriod)
                    {
                        var edition = (SubscribableEdition)await _editionManager.GetByIdAsync(input.EditionId.Value);
                        editionName = edition.DisplayName;
                        subscriptionEndDate = Clock.Now.AddDays(14);

                    }
                }


               

                input.TenancyName = input.TenancyName.Replace(" ", "");



                //c-here

                var tenantId = await _tenantManager.CreateWithAdminUserAsync(
                    
                    input.TenancyName, 
                    input.AdminPhoneNumber,
                    input.IsTwoFactorEnabled,
                    input.Name,
                    input.AdminPassword,
                    input.AdminEmailAddress,
                    null,
                    isActive: true,
                    input.EditionId,
                    shouldChangePasswordOnNextLogin: false,
                    sendActivationEmail: true,
                    subscriptionEndDate,
                    isInTrialPeriod,
                    input.NoOfUsers,
                    input.Storagelimit,
                    AppUrlService.CreateEmailActivationUrlFormat(input.TenancyName),
                    editionName, input.city,input.FiscalYearStart,input.FiscalYearEnd,input.state,input.country,input.Address,input.Zip_code,input.FirstName,input.lastName,input.Suffix
                  
                );

                var tenant = await _tenantManager.GetByIdAsync(tenantId);

                //await _appNotifier.NewTenantRegisteredAsync(tenant);

                return new RegisterTenantoutputDto
                {
                    TenantId = tenant.Id,
                    TenancyName = input.TenancyName,
                    Name = input.Name,
                    UserName = AbpUserBase.AdminUserName,
                    EmailAddress = input.AdminEmailAddress,
                    IsActive = tenant.IsActive,
                    IsEmailConfirmationRequired = isEmailConfirmationRequired,
                    IsTenantActive = tenant.IsActive,
                };
            }
        }
      
        private async Task<User> GetUserByChecking(string inputEmailAddress)
        {
            var user = await _userRepository.FirstOrDefaultAsync(a => a.EmailAddress.ToLower() == inputEmailAddress.ToLower());
            //if (user == null)
            //{
            //    throw new UserFriendlyException(L("InvalidEmailAddress"));
            //}

            return user;
        }
        private async Task CheckEditionSubscriptionAsync(int editionId, SubscriptionStartType subscriptionStartType)
        {
            var edition = await _editionManager.GetByIdAsync(editionId) as SubscribableEdition;

            CheckSubscriptionStart(edition, subscriptionStartType);
        }

        //private async Task CheckEditionSubscriptionAsync(int editionId, SubscriptionStartType subscriptionStartType)
        //{
        //    var edition = await _editionManager.GetByIdAsync(editionId) as SubscribableEdition;

        //    CheckSubscriptionStart(edition, subscriptionStartType);
        //}

        private static void CheckSubscriptionStart(SubscribableEdition edition, SubscriptionStartType subscriptionStartType)
        {
            switch (subscriptionStartType)
            {
                case SubscriptionStartType.Free:
                    if (!edition.IsFree)
                    {
                        throw new Exception("This is not a free edition !");
                    }
                    break;
                case SubscriptionStartType.Trial:
                    if (!edition.HasTrial())
                    {
                        throw new Exception("Trial is not available for this edition !");
                    }
                    break;
                case SubscriptionStartType.Paid:
                    if (edition.IsFree)
                    {
                        throw new Exception("This is a free edition and cannot be subscribed as paid !");
                    }
                    break;
            }
        }
        private async Task CheckRegistrationWithoutEdition()
        {
            //var editions = await _editionManager.GetAllAsync();
            //if (editions.Any())
            //{
            //    throw new Exception("Tenant registration is not allowed without edition because there are editions defined !");
            //}
        }
        private void CheckTenantRegistrationIsEnabled()
        {
            if (!IsSelfRegistrationEnabled())
            {
                throw new UserFriendlyException(L("SelfTenantRegistrationIsDisabledMessage_Detail"));
            }

            if (!_multiTenancyConfig.IsEnabled)
            {
                throw new UserFriendlyException(L("MultiTenancyIsNotEnabled"));
            }
        }
        private bool IsSelfRegistrationEnabled()
        {
            var result = SettingManager.GetSettingValueForApplication(AppSettingNames.TenantManagement.AllowSelfRegistration);
            return Convert.ToBoolean(result);
        }

        private async Task<bool> IsNewRegisteredTenantActiveByDefault(SubscriptionStartType subscriptionStartType)
        {
            if (subscriptionStartType == SubscriptionStartType.Paid)
            {
                return false;
            }
            if (subscriptionStartType == SubscriptionStartType.Free)
            {
                return true;
            }

            var result = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.TenantManagement.IsNewRegisteredTenantActiveByDefault);
            return Convert.ToBoolean(result);
        }










    }
}

