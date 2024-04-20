using AccountingBlueBook.AppServices.Emails.Dto;
using AccountingBlueBook.Entities.Main;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Emails.Dto
{
    public class EmailMapProfile : Profile
    {
        public EmailMapProfile()
        {
            CreateMap<EmailDto,Email>().ReverseMap();
            CreateMap<CreateOrEditEmailInputDto,Email>().ReverseMap();
        }
    }
}
