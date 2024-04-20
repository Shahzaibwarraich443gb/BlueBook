using Abp.Application.Editions;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace AccountingBlueBook.Payments
{
    public enum SubscriptionPaymentGatewayType
    {
        Paypal = 1,
        Stripe = 2
    }

    public enum SubscriptionPaymentStatus
    {
        NotPaid = 1,
        Paid = 2,
        Failed = 3,
        Cancelled = 4,
        Completed = 5
    }

    public enum PaymentPeriodType
    {
        Daily = 1,
        Weekly = 7,
        Monthly = 30,
        Annual = 365
    }

    public enum EditionPaymentType
    {
        /// <summary>
        /// Payment on first tenant registration.
        /// </summary>
        NewRegistration = 0,

        /// <summary>
        /// Purchasing by an existing tenant that currently using trial version of a paid edition.
        /// </summary>
        BuyNow = 1,

        /// <summary>
        /// A tenant is upgrading it's edition (either from a free edition or from a low-price paid edition).
        /// </summary>
        Upgrade = 2,

        /// <summary>
        /// A tenant is extending it's current edition (without changing the edition).
        /// </summary>
        Extend = 3
    }

    [Table("AppSubscriptionPayments")]
    [MultiTenancySide(MultiTenancySides.Host)]
    public class SubscriptionPayment : FullAuditedEntity<long>
    {
        public string Description { get; set; }

        public SubscriptionPaymentGatewayType Gateway { get; set; }

        public decimal Amount { get; set; }

        public SubscriptionPaymentStatus Status { get; protected set; }

        public int EditionId { get; set; }

        public int TenantId { get; set; }

        public int DayCount { get; set; }

        public PaymentPeriodType? PaymentPeriodType { get; set; }

        public string ExternalPaymentId { get; set; }

        public Edition Edition { get; set; }

        public string InvoiceNo { get; set; }

        public bool IsRecurring { get; set; }

        public string SuccessUrl { get; set; }

        public string ErrorUrl { get; set; }

        public EditionPaymentType EditionPaymentType { get; set; }

        public void SetAsCancelled()
        {
            if (Status == SubscriptionPaymentStatus.NotPaid)
            {
                Status = SubscriptionPaymentStatus.Cancelled;
            }
        }

        public void SetAsFailed()
        {
            Status = SubscriptionPaymentStatus.Failed;
        }

        public void SetAsPaid()
        {
            if (Status == SubscriptionPaymentStatus.NotPaid)
            {
                Status = SubscriptionPaymentStatus.Paid;
            }
        }

        public void SetAsCompleted()
        {
            if (Status == SubscriptionPaymentStatus.Paid)
            {
                Status = SubscriptionPaymentStatus.Completed;
            }
        }

        public SubscriptionPayment()
        {
            Status = SubscriptionPaymentStatus.NotPaid;
        }

        public PaymentPeriodType GetPaymentPeriodType()
        {
            switch (DayCount)
            {
                case 1:
                    return Payments.PaymentPeriodType.Daily;
                case 7:
                    return Payments.PaymentPeriodType.Weekly;
                case 30:
                    return Payments.PaymentPeriodType.Monthly;
                case 365:
                    return Payments.PaymentPeriodType.Annual;
                default:
                    throw new NotImplementedException($"PaymentPeriodType for {DayCount} day could not found");
            }
        }

        public bool IsProrationPayment()
        {
            return IsRecurring && EditionPaymentType == EditionPaymentType.Upgrade;
        }
    }

}
