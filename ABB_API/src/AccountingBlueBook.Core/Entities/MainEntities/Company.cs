// <copyright file="Company.cs" company="karma solutions"
// Copyright (c) karma solutions. All rights reserved.
// </copyright

namespace AccountingBlueBook.Entities.Main
{
    using Abp.Domain.Entities.Auditing;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Companies
    /// </summary>
    [Table("Companies")]
    public class Company : FullAuditedEntity
    {
        /// <summary>
        
        /// </summary>
        public string Name { get; set; }
        public string CompanyAddress { get; set; }
        public bool IsActive { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string Facebook { get; set; }
        public string LinkedIn { get; set; }

        public string Twitter { get; set; }
        public string Youtube { get; set; }
        public string GooglePlus { get; set; }

        public int? CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }

        public string Logo { get; set; }
        public string ak { get; set; }
        public string aps { get; set; }
        public string tk { get; set; }

        public int CheckStyle { get; set; }

        public bool UNSUBSCRIBE { get; set; }
        
        public System.DateTime FiscalYearStart { get; set; }
        public System.DateTime FiscalYearEnd { get; set; }
        /// <summary>
        /// </summary>
        public int? TenantId { get; set; }
      //  uhiuhiuhiuhiuhi
        public int ? EmailId { get; set; }
        [ForeignKey("EmailId")]
        public virtual Email Email { get; set; }    

        public int? PhoneId { get; set; }
        [ForeignKey("PhoneId")]
        public virtual Phone Phone { get; set; }

        public int? AddressId { get; set; }
        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }

        public int? PackageDetailId { get; set; }
        [ForeignKey("PackageDetailId")]
        public virtual PackageDetail PackageDetailFk { get; set; }

        public int? MerchantId { get; set; }
        [ForeignKey("MerchantId")]
        public virtual Merchant MerchantFk { get; set; }

        //public int? CountryId { get; set; }
        //[ForeignKey("CountryId")]
        //public virtual Country CountryFk { get; set; }


        public int? PackageTypeId { get; set; }
        [ForeignKey("PackageTypeId")]
        public virtual PackageType PackageType { get; set; }

    }
}
