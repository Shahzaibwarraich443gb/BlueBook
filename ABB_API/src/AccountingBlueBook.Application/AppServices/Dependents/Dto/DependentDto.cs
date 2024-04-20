using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Dependents.Dto
{
    public class DependentDto 
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Relation { get; set; }
        public string SSN { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
