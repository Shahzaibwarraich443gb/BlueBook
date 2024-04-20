using AccountingBlueBook.AppServices.Emails.Dto;
using AccountingBlueBook.AppServices.paymentMethod;
using AccountingBlueBook.AppServices.paymentMethod.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.usersGroup.Dto
{
    public  class UsersGroupMapProfile : Profile
    {
        public UsersGroupMapProfile()
        {
            CreateMap<GeneralUsersGroupDto, UsersGroup>().ReverseMap();
            CreateMap<CreateOrEditGeneralUserGroupInputDto, UsersGroup>().ReverseMap();
        }
    }
}
