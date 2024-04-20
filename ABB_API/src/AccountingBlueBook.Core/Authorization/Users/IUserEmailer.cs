using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.Authorization.Users
{
    public interface IUserEmailer
    {
        /// <summary>
        /// Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Email activation link</param>
        /// <param name="plainPassword">
        /// Can be set to user's plain password to include it in the email.
        /// </param>
        Task SendEmailActivationLinkAsync(User user, string link, string plainPassword = null);
        /// <summary>
        ///  Send Email Activation link to user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="link"></param>
        /// <param name="plainPassword"></param>
        /// <returns></returns>
        Task WelcomeEmail(User user, string link, string plainPassword = null);

        /// <summary>
        /// Sends a password reset link to user's email.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Password reset link (optional)</param>
        //Task SendPasswordResetLinkAsync(User user, string link = null);

        Task SubscriptionExpireEmail(int tenantId);
    }
}
