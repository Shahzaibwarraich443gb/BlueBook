using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using System;
using System.Collections.Generic;

namespace AccountingBlueBook.GeneralLedgers
{
    public class GeneralLedgerDto : FullAuditedEntityDto<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int CompanyId { get; set; }
        public long? CustomerId { get; set; }
        public int InvoiceType { get; set; }
        public string VoucherNo { get; set; }
        public long? CreatedBy { get; set; }
        public long InvoiceId { get; set; }
        public long ChartOfAccountId { get; set; }
        public string Title { get; set; }
        public double Balance { get; set; }
        public string CreatorUserName { get; set; }
        public Customer Customer { get; set; }

    }

    public class AddGeneralLedgarInputDto
    {
        public long InvoiceId { get; set; }
        public string ProcessType { get; set; }

    }

    public class GeneralLedgerOutputDto
    {
        public long Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string DateAlt { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string VoucherId { get; set; }
        public string CSR { get; set; }
        public string Description { get; set; }
        public double OpeningBalance { get; set; }
        public double DebitAmount { get; set; }
        public double CreditAmount { get; set; }
        public double Balance { get; set; }
        public string Type { get; set; }
        public long CustomerId { get; set; }
        public long? LinkedSubHeadId { get; set; }
        public List<GeneralLedgerChartOfAccountData> ChartOfAccountData { get; set; }
    }

    public class GetGeneralLedgerInputDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long? ChartOfAccountId { get; set; }
        public long? SubHeadId { get; set; }
    }

    public class GeneralLedgerChartOfAccountData
    {
        public long SubHeadId { get; set; }
        public string SubHeadName { get; set; }
        public long MainHeadId { get; set; }
        public string MainHeadName { get; set; }
        public double Amount { get; set; }
    }
}
