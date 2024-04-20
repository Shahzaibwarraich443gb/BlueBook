using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.EntityTypes.Dto
{
    public  class GeneralEntityTypeDto : EntityDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
