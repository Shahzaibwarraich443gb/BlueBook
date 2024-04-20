using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace AccountingBlueBook.Authorization
{
    public class AccountingBlueBookAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull(PermissionNames.Pages) ?? context.CreatePermission(PermissionNames.Pages, L("Modules"));
            var administration = pages.CreateChildPermission(PermissionNames.Pages_Administration, L("Administration"));

            #region  Customer
            var customer = administration.CreateChildPermission(PermissionNames.Pages_Customer, L("Customer"), multiTenancySides: MultiTenancySides.Tenant);
            customer.CreateChildPermission(PermissionNames.Pages_Customer_Create, L("Add Customer"));
            customer.CreateChildPermission(PermissionNames.Pages_Customer_Edit, L("Edit Customer"));
            customer.CreateChildPermission(PermissionNames.Pages_Customer_Delete, L("Delete Customer"));
            #endregion

            #region  All Sales
            var allSales = administration.CreateChildPermission(PermissionNames.Pages_AllSales, L("All Sales"), multiTenancySides: MultiTenancySides.Tenant);
           //salesTransaction
            var salesTransaction = allSales.CreateChildPermission(PermissionNames.Pages_AllSales_SalesTransaction, L("Sales Transaction"), multiTenancySides: MultiTenancySides.Tenant);
            salesTransaction.CreateChildPermission(PermissionNames.Pages_AllSales_SalesTransaction_Edit, L("Edit Transaction"));
            salesTransaction.CreateChildPermission(PermissionNames.Pages_AllSales_SalesTransaction_Delete, L("Delete Transaction"));
            //invoice
            var invoice = allSales.CreateChildPermission(PermissionNames.Pages_AllSales_Invoice, L("Invoice"), multiTenancySides: MultiTenancySides.Tenant);
            invoice.CreateChildPermission(PermissionNames.Pages_AllSales_Invoice_Create, L("Add Invoice"));
            invoice.CreateChildPermission(PermissionNames.Pages_AllSales_Invoice_Edit, L("Edit Invoice"));
            invoice.CreateChildPermission(PermissionNames.Pages_AllSales_Invoice_Delete, L("Delete Invoice"));
            //estimate
            var estimate = allSales.CreateChildPermission(PermissionNames.Pages_AllSales_Estimate, L("Estimate"), multiTenancySides: MultiTenancySides.Tenant);
            estimate.CreateChildPermission(PermissionNames.Pages_AllSales_Estimate_Create, L("Add Estimate"));
            estimate.CreateChildPermission(PermissionNames.Pages_AllSales_Estimate_Edit, L("Edit Estimate"));
            estimate.CreateChildPermission(PermissionNames.Pages_AllSales_Estimate_Delete, L("Delete Estimate"));
            //receivePayment
            var receivedPayment = allSales.CreateChildPermission(PermissionNames.Pages_AllSales_ReceivedPayment, L("ReceivedPayment"), multiTenancySides: MultiTenancySides.Tenant);
            receivedPayment.CreateChildPermission(PermissionNames.Pages_AllSales_ReceivedPayment_Create, L("Add ReceivedPayment"));
            receivedPayment.CreateChildPermission(PermissionNames.Pages_AllSales_ReceivedPayment_Edit, L("Edit ReceivedPayment"));
            receivedPayment.CreateChildPermission(PermissionNames.Pages_AllSales_ReceivedPayment_Delete, L("Delete ReceivedPayment"));
            //salesReceipt
            var salesReceipt = allSales.CreateChildPermission(PermissionNames.Pages_AllSales_SalesReceipt, L("SalesReceipt"), multiTenancySides: MultiTenancySides.Tenant);
            salesReceipt.CreateChildPermission(PermissionNames.Pages_AllSales_SalesReceipt_Create, L("Add SalesReceipt"));
            salesReceipt.CreateChildPermission(PermissionNames.Pages_AllSales_SalesReceipt_Edit, L("Edit SalesReceipt"));
            salesReceipt.CreateChildPermission(PermissionNames.Pages_AllSales_SalesReceipt_Delete, L("Delete SalesReceipt"));
            //creditNote
            var creditNote = allSales.CreateChildPermission(PermissionNames.Pages_AllSales_CreditNote, L("CreditNote"), multiTenancySides: MultiTenancySides.Tenant);
            creditNote.CreateChildPermission(PermissionNames.Pages_AllSales_CreditNote_Create, L("Add CreditNote"));
            creditNote.CreateChildPermission(PermissionNames.Pages_AllSales_CreditNote_Edit, L("Edit CreditNote"));
            creditNote.CreateChildPermission(PermissionNames.Pages_AllSales_CreditNote_Delete, L("Delete CreditNote"));
            //recurringInvoice
            var recurringInvoice = allSales.CreateChildPermission(PermissionNames.Pages_AllSales_RecurringInvoice, L("RecurringInvoice"), multiTenancySides: MultiTenancySides.Tenant);
            recurringInvoice.CreateChildPermission(PermissionNames.Pages_AllSales_RecurringInvoice_Create, L("Add RecurringInvoice"));
            recurringInvoice.CreateChildPermission(PermissionNames.Pages_AllSales_RecurringInvoice_Edit, L("Edit RecurringInvoice"));
            recurringInvoice.CreateChildPermission(PermissionNames.Pages_AllSales_RecurringInvoice_Delete, L("Delete RecurringInvoice"));
            #endregion

            #region Journal Voucher
            var journalVoucher = administration.CreateChildPermission(PermissionNames.Pages_JournalVoucher, L("JournalVoucher"), multiTenancySides: MultiTenancySides.Tenant);
            journalVoucher.CreateChildPermission(PermissionNames.Pages_JournalVoucher_Create, L("Add JournalVoucher"));
            journalVoucher.CreateChildPermission(PermissionNames.Pages_JournalVoucher_Edit, L("Edit JournalVoucher"));
            journalVoucher.CreateChildPermission(PermissionNames.Pages_JournalVoucher_Delete, L("Delete JournalVoucher"));
            #endregion

            #region All Expenses
            var allExpenses = administration.CreateChildPermission(PermissionNames.Pages_AllExpenses, L("All Expenses"), multiTenancySides: MultiTenancySides.Tenant);
            // purchaseReceipt
            var purchaseReceipt = allExpenses.CreateChildPermission(PermissionNames.Pages_AllExpense_PurchaseReceipt, L("PurchaseReceipt"), multiTenancySides: MultiTenancySides.Tenant);
            purchaseReceipt.CreateChildPermission(PermissionNames.Pages_AllExpense_PurchaseReceipt_Create, L("Add PurchaseReceipt"));
            purchaseReceipt.CreateChildPermission(PermissionNames.Pages_AllExpense_PurchaseReceipt_Edit, L("Edit PurchaseReceipt"));
            purchaseReceipt.CreateChildPermission(PermissionNames.Pages_AllExpense_PurchaseReceipt_Delete, L("Delete PurchaseReceipt"));
            // purchaseInvoice
            var purchaseInvoice = allExpenses.CreateChildPermission(PermissionNames.Pages_AllExpense_PurchaseInvoice, L("PurchaseInvoice"), multiTenancySides: MultiTenancySides.Tenant);
            purchaseInvoice.CreateChildPermission(PermissionNames.Pages_AllExpense_PurchaseInvoice_Create, L("Add PurchaseInvoice"));
            purchaseInvoice.CreateChildPermission(PermissionNames.Pages_AllExpense_PurchaseInvoice_Edit, L("Edit PurchaseInvoice"));
            purchaseInvoice.CreateChildPermission(PermissionNames.Pages_AllExpense_PurchaseInvoice_Delete, L("Delete PurchaseInvoice"));
            //purchasePayment
            var purchasePayment = allExpenses.CreateChildPermission(PermissionNames.Pages_AllExpense_PurchasePayment, L("PurchasePayment"), multiTenancySides: MultiTenancySides.Tenant);
            purchasePayment.CreateChildPermission(PermissionNames.Pages_AllExpense_PurchasePayment_Create, L("Add PurchasePayment"));
            purchasePayment.CreateChildPermission(PermissionNames.Pages_AllExpense_PurchasePayment_Edit, L("Edit PurchasePayment"));
            purchasePayment.CreateChildPermission(PermissionNames.Pages_AllExpense_PurchasePayment_Delete, L("Delete PurchasePayment"));
            //check
            var check = allExpenses.CreateChildPermission(PermissionNames.Pages_AllExpense_Check, L("Check"), multiTenancySides: MultiTenancySides.Tenant);
            check.CreateChildPermission(PermissionNames.Pages_AllExpense_Check_Create, L("Add Check"));
            check.CreateChildPermission(PermissionNames.Pages_AllExpense_Check_Edit, L("Edit Check"));
            check.CreateChildPermission(PermissionNames.Pages_AllExpense_Check_Delete, L("Delete Check"));
            #endregion

            #region Chart Of Accounts
            var chartOfAccount = administration.CreateChildPermission(PermissionNames.Pages_ChartOfAccount, L("ChartOfAccount"), multiTenancySides: MultiTenancySides.Tenant);
            chartOfAccount.CreateChildPermission(PermissionNames.Pages_ChartOfAccount_Create, L("Add ChartOfAccount"));
            chartOfAccount.CreateChildPermission(PermissionNames.Pages_ChartOfAccount_Edit, L("Edit ChartOfAccount"));
            chartOfAccount.CreateChildPermission(PermissionNames.Pages_ChartOfAccount_Delete, L("Delete ChartOfAccount"));
            #endregion

            #region  User Managemanet
            var userManagement = administration.CreateChildPermission(PermissionNames.Pages_UserManagement, L("User Management"), multiTenancySides: MultiTenancySides.Tenant);
            var users = userManagement.CreateChildPermission(PermissionNames.Pages_Users, L("Users"), multiTenancySides: MultiTenancySides.Tenant);
            var userGroup = userManagement.CreateChildPermission(PermissionNames.Pages_UsersGroup, L("Users Group"), multiTenancySides: MultiTenancySides.Tenant);
            var Roles = userManagement.CreateChildPermission(PermissionNames.Pages_Roles, L("Roles"), multiTenancySides: MultiTenancySides.Tenant);
            Roles.CreateChildPermission(PermissionNames.Pages_Roles_Create, L("Add Role"));
            Roles.CreateChildPermission(PermissionNames.Pages_Roles_Edit, L("Edit Role"));
            Roles.CreateChildPermission(PermissionNames.Pages_Roles_Delete, L("Delete Role"));
            #endregion

            // context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
            // context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            // context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AccountingBlueBookConsts.LocalizationSourceName);
        }
    }
}
