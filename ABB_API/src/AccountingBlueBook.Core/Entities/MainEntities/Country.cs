// <copyright file="Country.cs" company="PlaceholderCompany">
// Copyright (c) karma solutions. All rights reserved.
// </copyright>

namespace AccountingBlueBook.Entities.Main
{
    using Abp.Domain.Entities.Auditing;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Countries")]
    public class Country : FullAuditedEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
