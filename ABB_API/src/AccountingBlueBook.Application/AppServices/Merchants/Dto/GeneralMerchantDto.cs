using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Merchants.Dto
{
    public  class GeneralMerchantDto : EntityDto
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
