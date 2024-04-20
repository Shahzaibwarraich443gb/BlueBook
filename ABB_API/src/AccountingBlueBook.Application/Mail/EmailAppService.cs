using Abp.Net.Mail;
using Abp.UI;
using AccountingBlueBook;
using AccountingBlueBook.Authorization.Users;
using AccountingBlueBook.Authorization.Users.Dto;
using AccountingBlueBook.Emails;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Mail
{
    public class EmailAppService : AccountingBlueBookAppServiceBase, IEmailAppServices
    {
        private readonly EmailService _emailService;
        
        public EmailAppService(EmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task SendEmail()
        {
            await _emailService.SendEMail();
        }

        public async Task SendMail(EmailsDto input)
        {
            try
            {
                var subjectReplaced = input.Subject;
                var bodyReplaced = input.Body;
                var smtpPassword = await SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Password);
                var fromPassword = smtpPassword;
                var smtpUserName = await SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.UserName);
                var smtpPort = await SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Port);
                var smtpHost = await SettingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Host);
                var fromEmailAddress = await SettingManager.GetSettingValueAsync(EmailSettingNames.DefaultFromAddress);
                var fromDisplayName = await SettingManager.GetSettingValueAsync(EmailSettingNames.DefaultFromDisplayName);

                var client = new SmtpClient(smtpHost, int.Parse(smtpPort));
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUserName, fromPassword);
                client.EnableSsl = true;

                using (var mailMessage = new MailMessage($"AccountingBlueBook {fromEmailAddress}", input.ToEmail))
                {
                    mailMessage.Subject = subjectReplaced;
                    mailMessage.Body = bodyReplaced;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.BodyEncoding = Encoding.UTF8;

                    if (input.Streams != null)
                    {
                        foreach (var item in input.Streams)
                        {
                            var file = item.Item2;
                            file.Seek(0, SeekOrigin.Begin);
                            Attachment data = new Attachment(file, "application/pdf");
                            data.Name = item.Item1;
                            mailMessage.Attachments.Add(data);
                        }
                    }
                    await client.SendMailAsync(mailMessage);
                }
            }
            catch  (Exception ex) 
            {
                throw new UserFriendlyException("An error occurred while sending mail", ex.Message);
            }
        }
    }
}
