using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.paymentMethod.Dto
{
    public  class GeneralPaymentMethodDto : EntityDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
