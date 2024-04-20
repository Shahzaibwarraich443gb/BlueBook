using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using AccountingBlueBook.Authorization.Roles;
using AccountingBlueBook.Authorization.Users;
using AccountingBlueBook.MultiTenancy;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.NormalizeEntities;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using AccountingBlueBook.Entities.MainEntities.Vouchers;
using AccountingBlueBook.Entities.MainEntities.LinkedAccounts;
using System.Transactions;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AccountingBlueBook.Entities;

namespace AccountingBlueBook.EntityFrameworkCore
{
    public class AccountingBlueBookDbContext : AbpZeroDbContext<Tenant, Role, User, AccountingBlueBookDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public AccountingBlueBookDbContext(DbContextOptions<AccountingBlueBookDbContext> options)
            : base(options)
        {
        }

        public DbSet<CustomerUser> CustomerUsers { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<Dependent> Dependents { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<VenderType> VenderTypes { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankAddress> BankAddresses { get; set; }
        public DbSet<CustomerAddress> CustomerAddress { get; set; }
        public DbSet<CustomerPhoneNumber> CustomerPhoneNumber { get; set; }
        public DbSet<CustomerEmailAddress> CustomerEmailAddress { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<ProductService> ProductServices { get; set; }
        public DbSet<ChartOfAccount> ChartOfAccounts { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<VendorContactInfo> VendorContactInfos { get; set; }
        public DbSet<Spouse> Spouses { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<MainHead> MainHeads { get; set; }
        public DbSet<GeneralEntityType> GeneralEntityType { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<PackageDetail> PackagesDetail { get; set; }
        public DbSet<PackageType> PackageTypes { get; set; }
        public DbSet<PaymentPlan> PaymentPlans { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ContactPersonType> ContactPersonTypes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Ethnicity> Ethnicitys { get; set; }
        public DbSet<Language> Languags { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<SalesPersonType> SalesPersonTypes { get; set; }
        public DbSet<SourceReferralType> SourceReferralTypes { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<VoucherDetail> VoucherDetails { get; set; }

        public DbSet<LinkedAccount> LinkedAccounts { get; set; }

        public DbSet<PaymentMethod> PaymentMethod { get; set; }
        public DbSet<Entities.MainEntities.Transaction> Transactions { get; set; }

        public DbSet<DLState> DLStates { get; set; }

        public DbSet<CustomerPassword> CustomerPasswords { get; set; }
        public DbSet<CardTransaction> CardTransactions { get; set; }

        public DbSet<PaymentTermList> paymentTermLists { get; set; }

        public DbSet<ChartOfAccountMaster> chartOfAccountMasters { get; set; }

        public DbSet<CustomerDiary> CustomerDiary { get; set; }

        public DbSet<CustomerCard> CustomerCard { get; set; }
        public DbSet<LegalStatus> LegalStatuses { get; set; }
        public DbSet<TenureForm> TenureForms { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<SalesTax> SalesTaxes { get; set; }

        public DbSet<CustomerTaxSelection> CustomerTaxSelections { get; set; }
        public DbSet<PersonalTax> PersonalTax { get; set; }
        public DbSet<IncomeDetails> IncomeDetails { get; set; }

        public DbSet<CorporateTax> CorporateTax { get; set; }

        public DbSet<CustomerCard> CustomerCards { get; set; }

        public DbSet<RecurringInvoice> RecurringInvoices { get; set;  }  
        public DbSet<Employee> Employees { get; set; }
       
        public DbSet<Check> Checks { get; set; }
        public DbSet<CheckAccountDetail> CheckAccountDetails {get; set;}
        public DbSet<CheckProductDetail> CheckProductDetails {get; set;}
        public DbSet<CheckSetup> CheckSetups { get; set; }  
        public DbSet<UsersGroup> UserGroup { get; set; }
        public DbSet<VendorAddress> VendorAddresses { get; set; } 
        public DbSet<GeneralLedger> GeneralLedgers { get; set; }
        public DbSet<GeneralLedgerDetails> GeneralLedgerDetails { get; set; }
        public DbSet<VendorAttachment> VendorAttachments { get; set; }
        public DbSet<LedgerHeaders> LedgerHeaders { get; set; }
    }
}

