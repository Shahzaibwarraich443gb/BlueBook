using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Security;
using Abp.UI;
using Abp.Zero.Configuration;
using AccountingBlueBook.Authorization.Accounts.Dto;
using AccountingBlueBook.Authorization.Users;
using AccountingBlueBook.MultiTenancy;
using AccountingBlueBook.TwilioConfiguratrions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace AccountingBlueBook.Authorization.Accounts
{
    public class AccountAppService : AccountingBlueBookAppServiceBase, IAccountAppService
    {
        // from: http://regexlib.com/REDetails.aspx?regexp_id=1923
        public const string PasswordRegex = "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\\s)[0-9a-zA-Z!@#$%^&*()]*$";

        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly ITwilioConfiguration _twilioConfiguration;
        public AccountAppService(
            UserRegistrationManager userRegistrationManager,
            ITwilioConfiguration twilioConfiguration,
            IRepository<User, long> userRepository)
        {
            _userRegistrationManager = userRegistrationManager;
            _userRepository = userRepository;
            _twilioConfiguration = twilioConfiguration;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            Tenant tenant;
            int? tenantId;
            if(!input.EmailAddress.IsNullOrEmpty())
            {
                tenantId = await _userRepository.GetAll()
                                                    .IgnoreQueryFilters()
                                                    .Where(a => !a.IsDeleted && (a.EmailAddress.ToLower() == input.EmailAddress.ToLower()))
                                                    .Select(a => a.TenantId)
                                                    .FirstOrDefaultAsync();
                if (tenantId.HasValue)
                {
                    tenant = await TenantManager.GetByIdAsync((int)tenantId);
                    return await CheckTenantAvailability(tenant);
                }
                else
                {
                    tenant = null;
                    return await CheckTenantAvailability(tenant);
                }
            }
            else
            {
                tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
                return await CheckTenantAvailability(tenant);

            }
            
        }

        private async Task<IsTenantAvailableOutput> CheckTenantAvailability(Tenant tenant)
        {
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                //return new IsTenantAvailableOutput(TenantAvailabilityState.InActive, tenant.Id, !tenant.IsInTrialPeriod ? true : false);
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive, tenant.Id, true);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.Surname,
                input.EmailAddress,
                input.UserName,
                input.Password,
                true // Assumed email address is always confirmed. Change this if you want to implement email confirmation.
            );

            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }

        public Task<int?> ResolveTenantId(ResolveTenantIdInput input)
        {
            if (string.IsNullOrEmpty(input.c))
            {
                return Task.FromResult(AbpSession.TenantId);
            }


            var parameters = SimpleStringCipher.Instance.Decrypt(input.c);
            var query = HttpUtility.ParseQueryString(parameters);

            if (query["tenantId"] == null)
            {
                return Task.FromResult<int?>(null);
            }

            var tenantId = Convert.ToInt32(query["tenantId"]) as int?;
            return Task.FromResult(tenantId);
        }

           public async Task ActivateEmail(ActivateEmailInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            if (user != null && user.IsEmailConfirmed)
            {
                return;
            }

            if (user == null || user.EmailConfirmationCode.IsNullOrEmpty() || user.EmailConfirmationCode != input.ConfirmationCode)
            {
                throw new UserFriendlyException(L("InvalidEmailConfirmationCode"), L("InvalidEmailConfirmationCode_Detail"));
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = null;

            await UserManager.UpdateAsync(user);
        }
        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        [HttpGet]
        public async Task<string> SendOTP(string phoneNumber)
        {
            //Auther Dawood
            // Find your Account SID and Auth Token at twilio.com/console
            // and set the environment variables. See http://twil.io/secure
            //Trial credentials         
            ////abc testing changes are not fetched on the server checking .
            //string accountSid = "ACe69c41abfdc00159770ef283d3ed4e12";
            //string authToken = "bbee817a455c36e0350e45003de3cd80";
            //string fromPhoneNumber = "+16065521739";

            //string accountSid = _twilioConfiguration.accountSid;
            //string authToken = _twilioConfiguration.authToken;
            //string fromPhoneNumber = _twilioConfiguration.fromPhoneNumber;
            TwilioClient.Init(_twilioConfiguration.accountSid, _twilioConfiguration.authToken);
            string OTP = "" + GenerateRandomNo();
            var message = MessageResource.Create(
                body: "OTP for AccountingBlueBook is: " + OTP,
                from: new Twilio.Types.PhoneNumber(_twilioConfiguration.fromPhoneNumber),
                to: new Twilio.Types.PhoneNumber(phoneNumber)
            );
            return OTP;
        }

    }
}
