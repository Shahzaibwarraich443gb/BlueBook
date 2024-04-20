// <copyright file="PackageType.cs" company="karma solutions">
// Copyright (c) karma solutions. All rights reserved.
// </copyright>

namespace AccountingBlueBook.Entities.Main
{
    using Abp.Domain.Entities.Auditing;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PackageTypes")]
    public class PackageType : FullAuditedEntity
    {
        public string Title { get; set; }
        public string Duration { get; set; }
        public string Note { get; set; }

        public int? PaymentPlanId { get; set; }
        [ForeignKey("PaymentPlanId")]
        public PaymentPlan PaymentPlan { get; set; }
    }
}
