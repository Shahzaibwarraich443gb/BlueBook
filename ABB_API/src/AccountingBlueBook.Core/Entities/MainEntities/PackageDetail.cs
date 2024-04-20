// <copyright file="PackageDetail.cs" company="karma solutions">
// Copyright (c) karma solutions. All rights reserved.
// </copyright>

namespace AccountingBlueBook.Entities.Main
{
    using Abp.Domain.Entities.Auditing;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PackageDetails")]
    public class PackageDetail : FullAuditedEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }

        public int? PackageTypeId { get; set; }
        [ForeignKey("PackageTypeId")]
        public virtual PackageType PackageType { get; set; }
    }
}
