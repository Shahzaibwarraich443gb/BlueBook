// <copyright file="Currency.cs" company="PlaceholderCompany">
// Copyright (c) karma solutions. All rights reserved.
// </copyright>

namespace AccountingBlueBook.Entities.Main
{
    using Abp.Domain.Entities.Auditing;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Currencies")]
    public class Currency : FullAuditedEntity
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
    }
}
