using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.VendorContactInfos.Dto
{
    public  class VendorContactInfoDto : EntityDto
    {
        public string ContactPersonName { get; set; }
        public string ContactTypeName { get; set; }
        public string Fax { get; set; }
        public int? EmailTypeId { get; set; }
        public string EFax { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string WebSite { get; set; }
        public bool Primary { get; set; }
        public int? ContactPersonTypeId { get; set; }
        public int? ContactTypeId { get; set; }
        public int? VendorId { get; set; }
        public ContactPersonType ContactPersonType { get; set; }

        public ContactTypeEnum ContactTypeEnum { get; set; }
    }
}
