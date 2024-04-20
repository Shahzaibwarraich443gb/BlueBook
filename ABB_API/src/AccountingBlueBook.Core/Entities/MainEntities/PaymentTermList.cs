// <copyright file="PaymentPlan.cs" company="karma solutions">
// Copyright (c) karma solutions. All rights reserved.
// </copyright>

namespace AccountingBlueBook.Entities.Main
{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PaymentTermList")]
    public class PaymentTermList : FullAuditedEntity<long>
    {
        public string Description { get; set; }
        public Nullable<int> Days { get; set; }
    }
}
