// <copyright file="PaymentPlan.cs" company="karma solutions">
// Copyright (c) karma solutions. All rights reserved.
// </copyright>

namespace AccountingBlueBook.Entities.Main
{
    using Abp.Domain.Entities.Auditing;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PaymentPlans")]
    public class PaymentPlan : FullAuditedEntity
    {
        public string Duration { get; set; }
        public string Note { get; set; }
    }
}
