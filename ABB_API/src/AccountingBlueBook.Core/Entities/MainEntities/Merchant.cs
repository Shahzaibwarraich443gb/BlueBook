// <copyright file="Merchant.cs" company="PlaceholderCompany">
// Copyright (c) karma solutions. All rights reserved.
// </copyright>

namespace AccountingBlueBook.Entities.Main
{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Merchants")]
    public class Merchant : FullAuditedEntity
    {
        public string Name { get; set; }
        public string APIKey { get; set; }
        public string APISecretKey { get; set; }
        public string Token { get; set; }
        public string CCMID { get; set; }
        public string CCUN { get; set; }
        public string CCPWD { get; set; }
        public bool Active { get; set; }
       
       
    }
}
