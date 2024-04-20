using Abp.Configuration;
using Abp.Dependency;
using Abp.Net.Mail;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System.Net;
using System.Threading.Tasks;

namespace AccountingBlueBook.Emails
{
    public class EmailService : AccountingBlueBookAppServiceBase, ITransientDependency
    {
        public async Task SendEMail()
        {

            var smtpPassword = await SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Password);
            var fromPassword = smtpPassword;
            var smtpUserName = await SettingManager.GetSettingValueAsync(Abp.Net.Mail.EmailSettingNames.Smtp.UserName);
            var smtpPort = await SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Port);
            var smtpHost = await SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Host);
            var fromEmailAddress = await SettingManager.GetSettingValueAsync(EmailSettingNames.DefaultFromAddress);
            var fromDisplayName = await SettingManager.GetSettingValueAsync(EmailSettingNames.DefaultFromDisplayName);
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("from name", smtpUserName));
            mailMessage.To.Add(new MailboxAddress("to name", "dawoodonline19@gmail.com"));
            mailMessage.Subject = "subject";
            mailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = "<b>Hello</b>"
            };

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync("smtp.gmail.com", 587, true);
                await smtpClient.AuthenticateAsync(smtpUserName, fromPassword);
                await smtpClient.SendAsync(mailMessage);
                await smtpClient.DisconnectAsync(true);
            }

        }
    }
}
