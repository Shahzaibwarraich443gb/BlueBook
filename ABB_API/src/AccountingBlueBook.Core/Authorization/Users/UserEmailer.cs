using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Runtime.Security;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AccountingBlueBook.Editions;
using AccountingBlueBook.Authorization.Users;
using AccountingBlueBook.MultiTenancy;
using AccountingBlueBook.Net.Emailing;
using AccountingBlueBook;
using AccountingBlueBook.Authorization.Users.Dto;

namespace AccountingBlueBook.Authorization.Users
{
    public class UserEmailer : AccountingBlueBook_Web_ApplicationServiceBase, IUserEmailer, ITransientDependency
    {
        //private readonly IConfigurationRoot _appConfiguration;

        private readonly IEmailTemplateProvider _emailTemplateProvider;
        //private readonly IEmailSender _emailSender;

        private readonly IRepository<User, long> _userRepository;
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IEmailAppServices _emailAppService;
        private readonly EditionManager _editionManager;
        private readonly UserManager _userManager;

        private string _emailButtonStyle =
            "padding-left: 30px; padding-right: 30px; padding-top: 12px; padding-bottom: 12px; color: #ffffff; background-color: #00bb77; font-size: 14pt; text-decoration: none;";
        private string _emailButtonColor = "#00bb77";

        //public string ServerRootAddress => _appConfiguration["App:ServerRootAddress"];

        public UserEmailer(ICurrentUnitOfWorkProvider unitOfWorkProvider,
                           //IEmailSender emailSender,
                           EditionManager editionManager,
                           UserManager userManager,
                           IRepository<Tenant> tenantRepository,
                           IEmailTemplateProvider emailTemplateProvider,
                           IEmailAppServices emailAppService,
                           //IAppConfigurationAccessor configurationAccessor,
                           IRepository<User, long> userRepository)
        {
            _emailTemplateProvider = emailTemplateProvider;
            //_emailSender = emailSender;
            _unitOfWorkProvider = unitOfWorkProvider;
            _editionManager = editionManager;
            _userManager = userManager;
            _tenantRepository = tenantRepository;
            _emailAppService = emailAppService;
            //_appConfiguration = configurationAccessor.Configuration;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Email activation link</param>
        /// <param name="plainPassword">
        /// Can be set to user's plain password to include it in the email.
        /// </param>
        [UnitOfWork]
        public virtual async Task SendEmailActivationLinkAsync(User user, string link, string plainPassword = null)
        {
            if (user.EmailConfirmationCode.IsNullOrEmpty())
            {
                throw new Exception("EmailConfirmationCode should be set in order to send email activation link.");
            }

            link = link.Replace("{userId}", user.Id.ToString());
            link = link.Replace("{confirmationCode}", Uri.EscapeDataString(user.EmailConfirmationCode));

            if (user.TenantId.HasValue)
            {
                link = link.Replace("{tenantId}", user.TenantId.ToString());
            }

            link = EncryptQueryParameters(link);

            var tenancyName = GetTenancyNameOrNull(user.TenantId);
            var emailTemplate = GetTitleAndSubTitle(user.TenantId, "EmailActivation_Title", "EmailActivation_SubTitle");
            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + "NameSurname" + "</b>: " + user.Name + " " + user.Surname + "<br />");

            if (!tenancyName.Item1.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + "TenancyName" + "</b>: " + tenancyName + "<br />");
            }
            if (!tenancyName.Item2.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + "TenantKey" + "</b>: " + tenancyName.Item2 + "<br />");
            }

            mailMessage.AppendLine("<b>" + "UserName" + "</b>: " + user.UserName + "<br />");

            if (!plainPassword.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + "Password" + "</b>: " + plainPassword + "<br />");
            }

            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("EmailActivation_ClickTheLinkBelowToVerifyYourEmail" + "<br /><br />");
            mailMessage.AppendLine("<a style=\"" + _emailButtonStyle + "\" bg-color=\"" + _emailButtonColor + "\" href=\"" + link + "\">" + "Verify" + "</a>");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<span style=\"font-size: 9pt;\">" + "EmailMessage_CopyTheLinkBelowToYourBrowser" + "</span><br />");
            mailMessage.AppendLine("<span style=\"font-size: 8pt;\">" + link + "</span>");

            await ReplaceBodyAndSend(user.EmailAddress, "EmailActivation_Subject", emailTemplate, mailMessage);
        }

        [UnitOfWork]
        public virtual async Task WelcomeEmail(User user, string link, string plainPassword = null)
        {
            if (user.EmailConfirmationCode.IsNullOrEmpty())
            {
                throw new Exception("EmailConfirmationCode should be set in order to send email activation link.");
            }

            link = link.Replace("{userId}", user.Id.ToString());
            link = link.Replace("{confirmationCode}", Uri.EscapeDataString(user.EmailConfirmationCode));

            if (user.TenantId.HasValue)
            {
                link = link.Replace("{tenantId}", user.TenantId.ToString());
            }

            link = EncryptQueryParameters(link);

            var tenancyName = GetTenancyNameOrNull(user.TenantId);
            var emailTemplate = GetTitleAndSubTitle(user.TenantId, "Welcome To AccountingBlueBook", string.Empty);
            var mailMessage = new StringBuilder();

            //mailMessage.AppendLine("<b>" + "Email" + "</b>: " + user.EmailAddress +  "<br />");

            //if (!tenancyName.Item1.IsNullOrEmpty())
            //{
            //    mailMessage.AppendLine("<b>" + "TenancyName" + "</b>: " + tenancyName.Item1 + "<br />");
            //}
            //if (!tenancyName.Item2.IsNullOrEmpty())
            //{
            //    mailMessage.AppendLine("<b>" + "TenantKey" + "</b>: " + tenancyName.Item2 + "<br />");
            //}

            //mailMessage.AppendLine("<b>" + "UserName" + "</b>: " + user.UserName + "<br />");

            //if (!plainPassword.IsNullOrEmpty())
            //{
            //    mailMessage.AppendLine("<b>" + "Password" + "</b>: " + plainPassword + "<br />");
            //}

            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("Email Activation: Click the link below to verify your email" + "<br /><br />");
            mailMessage.AppendLine("<a style=\"" + _emailButtonStyle + "\" bg-color=\"" + _emailButtonColor + "\" href=\"" + link + "\">" + 
                "Verify" + "</a>");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<span style=\"font-size: 9pt;\">" + "Email Message: Copy the link below to your browser" + "</span><br />");
            mailMessage.AppendLine("<span style=\"font-size: 8pt;\">" + link + "</span>");

            await ReplaceBodyAndSend(user.EmailAddress, "EmailActivation_Subject", emailTemplate, mailMessage);
        }

        [UnitOfWork]
        public virtual async Task SubscriptionExpireEmail(int tenantId)
        {
            User user;
            user = await _userRepository.FirstOrDefaultAsync(a => a.UserName.StartsWith("admin_") && a.TenantId == tenantId);
            user = await _userRepository.GetAll().IgnoreQueryFilters().FirstOrDefaultAsync(a => a.UserName.StartsWith("admin_") && a.TenantId == tenantId);

            var tenancyName = GetTenancyNameOrNull(user.TenantId);
            var emailTemplate = GetTitleAndSubTitle(user.TenantId, "Important Email", string.Empty);
            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + "Tenant" + "</b>: " + tenancyName.Item1 + " is expired" + "<br />");

            await ReplaceBodyAndSend(user.EmailAddress, "Tenant Expire", emailTemplate, mailMessage);
        }

        private string EncryptQueryParameters(string link, string encrptedParameterName = "c")
        {
            if (!link.Contains("?"))
            {
                return link;
            }

            var basePath = link.Substring(0, link.IndexOf('?'));
            var query = link.Substring(link.IndexOf('?')).TrimStart('?');

            return basePath + "?" + encrptedParameterName + "=" + HttpUtility.UrlEncode(SimpleStringCipher.Instance.Encrypt(query));
        }

        private Tuple<string,string> GetTenancyNameOrNull(int? tenantId)
        {
            if (tenantId == null)
            {
                return null;
            }

            using (_unitOfWorkProvider.Current.SetTenantId(null))
            {
                var tenant = _tenantRepository.Get(tenantId.Value);
                return new Tuple<string, string>(tenant.TenancyName, tenant.TenantKey.ToString());
            }
        }

        private StringBuilder GetTitleAndSubTitle(int? tenantId, string title, string subTitle)
        {
            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(tenantId));
            emailTemplate.Replace("{EMAIL_TITLE}", title);
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", subTitle);

            return emailTemplate;
        }

        private async Task ReplaceBodyAndSend(string emailAddress, string subject, StringBuilder emailTemplate, StringBuilder mailMessage)
        {
            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());

            var mail = new EmailsDto()
            {
                Body = emailTemplate.ToString(),
                Subject = subject,
                ToEmail = emailAddress
            };
            await _emailAppService.SendMail(mail);
            
        }

    }
}
