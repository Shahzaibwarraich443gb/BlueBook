using Abp.Application.Editions;
using AccountingBlueBook.Payments;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingBlueBook.Editions
{
    public enum EndSubscriptionResult
    {
        TenantSetInActive,
        AssignedToAnotherEdition
    }
    public class SubscribableEdition : Edition
    {
        /// <summary>
        /// The edition that will assigned after expire date
        /// </summary>
        public int? ExpiringEditionId { get; set; }

        public decimal? DailyPrice { get; set; }

        public decimal? WeeklyPrice { get; set; }

        public decimal? MonthlyPrice { get; set; }

        public decimal? AnnualPrice { get; set; }

        public int? TrialDayCount { get; set; }

        public decimal? PerEmployeeCost { get; set; }

 

        /// <summary>
        /// The account will be taken an action (termination of tenant account) after the specified days when the subscription is expired.
        /// </summary>
        public int? WaitingDayAfterExpire { get; set; }
        public bool ShowInEditionPage { get; set; }
        public int Sequence { get; set; }

        [NotMapped]
        public bool IsFree => !DailyPrice.HasValue && !WeeklyPrice.HasValue && !MonthlyPrice.HasValue && !AnnualPrice.HasValue;

        public bool HasTrial()
        {
            if (IsFree)
            {
                return false;
            }

            return TrialDayCount.HasValue && TrialDayCount.Value > 0;
        }

        public decimal GetPaymentAmount(PaymentPeriodType? paymentPeriodType, int users = 0, int space = 0)
        {
            var amount = CalPaymentAmountOrNull(paymentPeriodType, users, space);
            //var amount = GetPaymentAmountOrNull(paymentPeriodType);
            if (!amount.HasValue)
            {
                throw new Exception("No price information found for " + DisplayName + " edition!");
            }

            return amount.Value;
        }

        public decimal? GetPaymentAmountOrNull(PaymentPeriodType? paymentPeriodType)
        {
            switch (paymentPeriodType)
            {
                case PaymentPeriodType.Daily:
                    return DailyPrice;
                case PaymentPeriodType.Weekly:
                    return WeeklyPrice;
                case PaymentPeriodType.Monthly:
                    return MonthlyPrice;
                case PaymentPeriodType.Annual:
                    return AnnualPrice;
                default:
                    return null;
            }
        }

        public decimal? CalPaymentAmountOrNull(PaymentPeriodType? paymentPeriodType, int users, int space)
        {
            if (paymentPeriodType == PaymentPeriodType.Monthly)
            {
                return CalMonthlyPrice(users, space);
            }

            if (paymentPeriodType == PaymentPeriodType.Annual)
            {
                return CalMonthlyPrice(users, space) * 12;
                //return ((PerUserCost * users) + (PerGbCost * space)) * 12 + 20;
            }
            return null;
        }

        private decimal CalMonthlyPrice(int users, int space)
        {
            return 20;//(PerUserCost * users) + (PerGbCost * (space - users)) + 20;
        }

    }
}
