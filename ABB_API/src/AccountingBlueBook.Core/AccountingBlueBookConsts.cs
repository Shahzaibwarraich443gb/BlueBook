using AccountingBlueBook.Debugging;

namespace AccountingBlueBook
{
    public class AccountingBlueBookConsts
    {
        public const string LocalizationSourceName = "AccountingBlueBook";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;

        public const bool AllowTenantsToChangeEmailSettings = false;

        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "957c98e50a6e4031a08f71ebe478a922";
    }
}
