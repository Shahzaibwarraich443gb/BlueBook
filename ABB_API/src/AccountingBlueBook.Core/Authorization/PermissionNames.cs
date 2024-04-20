namespace AccountingBlueBook.Authorization
{
    public static class PermissionNames
    {
        public const string Pages = "Pages";
        public const string Pages_Administration = "Pages.Administration";       

        /* Customer */
        public const string Pages_Customer = "Pages.Customer";
        public const string Pages_Customer_Create = "Pages.Customer.Create";
        public const string Pages_Customer_Edit = "Pages.Customer.Edit";
        public const string Pages_Customer_Delete = "Pages.Customer.Delete";

        #region  All Sales
        /* All Sales */
        public const string Pages_AllSales = "Pages.AllSales";
        // SalesTransaction
        public const string Pages_AllSales_SalesTransaction = "Pages.AllSales.SalesTransaction";
        public const string Pages_AllSales_SalesTransaction_Create = "Pages.AllSales.SalesTransaction.Create";
        public const string Pages_AllSales_SalesTransaction_Edit = "Pages.AllSales.SalesTransaction.Edit";
        public const string Pages_AllSales_SalesTransaction_Delete = "Pages.AllSales.SalesTransaction.Delete";
        // Invoice
        public const string Pages_AllSales_Invoice = "Pages.AllSales.Invoice";
        public const string Pages_AllSales_Invoice_Create = "Pages.AllSales.Invoice.Create";
        public const string Pages_AllSales_Invoice_Edit = "Pages.AllSales.Invoice.Edit";
        public const string Pages_AllSales_Invoice_Delete = "Pages.AllSales.Invoice.Delete";
        // Estimate
        public const string Pages_AllSales_Estimate = "Pages.AllSales.Estimate";
        public const string Pages_AllSales_Estimate_Create = "Pages.AllSales.Estimate.Create";
        public const string Pages_AllSales_Estimate_Edit = "Pages.AllSales.Estimate.Edit";
        public const string Pages_AllSales_Estimate_Delete = "Pages.AllSales.Estimate.Delete";
        // ReceivedPayment
        public const string Pages_AllSales_ReceivedPayment = "Pages.AllSales.ReceivedPayment";
        public const string Pages_AllSales_ReceivedPayment_Create = "Pages.AllSales.ReceivedPayment.Create";
        public const string Pages_AllSales_ReceivedPayment_Edit = "Pages.AllSales.ReceivedPayment.Edit";
        public const string Pages_AllSales_ReceivedPayment_Delete = "Pages.AllSales.ReceivedPayment.Delete";
        // SalesReceipt
        public const string Pages_AllSales_SalesReceipt = "Pages.AllSales.SalesReceipt";
        public const string Pages_AllSales_SalesReceipt_Create = "Pages.AllSales.SalesReceipt.Create";
        public const string Pages_AllSales_SalesReceipt_Edit = "Pages.AllSales.SalesReceipt.Edit";
        public const string Pages_AllSales_SalesReceipt_Delete = "Pages.AllSales.SalesReceipt.Delete";
        // CreditNote
        public const string Pages_AllSales_CreditNote = "Pages.AllSales.CreditNote";
        public const string Pages_AllSales_CreditNote_Create = "Pages.AllSales.CreditNote.Create";
        public const string Pages_AllSales_CreditNote_Edit = "Pages.AllSales.CreditNote.Edit";
        public const string Pages_AllSales_CreditNote_Delete = "Pages.AllSales.CreditNote.Delete";
        // RecurringInvoice
        public const string Pages_AllSales_RecurringInvoice = "Pages.AllSales.RecurringInvoice";
        public const string Pages_AllSales_RecurringInvoice_Create = "Pages.AllSales.RecurringInvoice.Create";
        public const string Pages_AllSales_RecurringInvoice_Edit = "Pages.AllSales.RecurringInvoice.Edit";
        public const string Pages_AllSales_RecurringInvoice_Delete = "Pages.AllSales.RecurringInvoice.Delete";
        #endregion

        #region Journal Voucher
        public const string Pages_JournalVoucher = "Pages.JournalVoucher";
        public const string Pages_JournalVoucher_Create = "Pages.JournalVoucher.Create";
        public const string Pages_JournalVoucher_Edit = "Pages.JournalVoucher.Edit";
        public const string Pages_JournalVoucher_Delete = "Pages.JournalVoucher.Delete";
        #endregion

        #region All Expenses
        public const string Pages_AllExpenses = "Pages.AllExpenses";
        // PurchaseReceipt
        public const string Pages_AllExpense_PurchaseReceipt = "Pages.AllExpense.PurchaseReceipt";
        public const string Pages_AllExpense_PurchaseReceipt_Create = "Pages.AllExpense.PurchaseReceipt.Create";
        public const string Pages_AllExpense_PurchaseReceipt_Edit = "Pages.AllExpense.PurchaseReceipt.Edit";
        public const string Pages_AllExpense_PurchaseReceipt_Delete = "Pages.AllExpense.PurchaseReceipt.Delete";
        // PurchaseInvoice
        public const string Pages_AllExpense_PurchaseInvoice = "Pages.AllExpense.PurchaseInvoice";
        public const string Pages_AllExpense_PurchaseInvoice_Create = "Pages.AllExpense.PurchaseInvoice.Create";
        public const string Pages_AllExpense_PurchaseInvoice_Edit = "Pages.AllExpense.PurchaseInvoice.Edit";
        public const string Pages_AllExpense_PurchaseInvoice_Delete = "Pages.AllExpense.PurchaseInvoice.Delete";
        // PurchasePayment
        public const string Pages_AllExpense_PurchasePayment = "Pages.AllExpense.PurchasePayment";
        public const string Pages_AllExpense_PurchasePayment_Create = "Pages.AllExpense.PurchasePayment.Create";
        public const string Pages_AllExpense_PurchasePayment_Edit = "Pages.AllExpense.PurchasePayment.Edit";
        public const string Pages_AllExpense_PurchasePayment_Delete = "Pages.AllExpense.PurchasePayment.Delete";
        // Check
        public const string Pages_AllExpense_Check = "Pages.AllExpense.Check";
        public const string Pages_AllExpense_Check_Create = "Pages.AllExpense.Check.Create";
        public const string Pages_AllExpense_Check_Edit = "Pages.AllExpense.Check.Edit";
        public const string Pages_AllExpense_Check_Delete = "Pages.AllExpense.Check.Delete";
        #endregion

        #region ChartOfAccount
        public const string Pages_ChartOfAccount = "Pages.ChartOfAccount";
        public const string Pages_ChartOfAccount_Create = "Pages.ChartOfAccount.Create";
        public const string Pages_ChartOfAccount_Edit = "Pages.ChartOfAccount.Edit";
        public const string Pages_ChartOfAccount_Delete = "Pages.ChartOfAccount.Delete";
        #endregion

        #region  User Management
        public const string Pages_UserManagement = "Pages.UserManagement";
        /* Role */
        public const string Pages_Roles = "Pages.Roles";
        public const string Pages_Roles_Create = "Pages.Roles.Create";
        public const string Pages_Roles_Edit = "Pages.Roles.Edit";
        public const string Pages_Roles_Delete = "Pages.Roles.Delete";
        // users
        public const string Pages_Users = "Pages.Users";
        // usersGroup
        public const string Pages_UsersGroup = "Pages.UsersGroup";
        #endregion

        public const string Pages_Tenants = "Pages.Tenants";
        public const string Pages_Users_Activation = "Pages.Users.Activation";

        //public const string Pages_Roles = "Pages.Roles";
        //  public const string Pages_Users = "Pages.Users";

    }
}
