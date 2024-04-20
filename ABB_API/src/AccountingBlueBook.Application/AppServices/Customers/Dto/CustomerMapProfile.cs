using Abp.AutoMapper;
using AccountingBlueBook.AppServices.Adresses.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Customers.Dto
{
    public class CustomerMapProfile : Profile
    {
        public CustomerMapProfile()
        {
            CreateMap<CustomerAddressDto, Address>()
                .ForMember(x => x.Customer, opt => opt.Ignore())
                .ForMember(x => x.CompleteAddress, opt => opt.MapFrom(x => x.Address))
                .ForMember(x => x.PostCode, opt => opt.MapFrom(x => x.Zip))
                .ReverseMap();
            CreateMap<Address, CustomerAddressDto>()
                .ForMember(x => x.Address, opt => opt.MapFrom(x => x.CompleteAddress))
                .ForMember(x => x.Zip, opt => opt.MapFrom(x => x.PostCode))
                .ReverseMap();
            CreateMap<ContactInfoDto, ContactInfo>()
                .ForMember(x => x.Customer, opt => opt.Ignore())
                .ForMember(x => x.ContactInfoType, options => options.MapFrom(l => l.ContactType))
                .ReverseMap();
            CreateMap<UserNamePasswordDto, CustomerUser>()
                .ForMember(x => x.Customer, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<CustomerUser, UserNamePasswordDto>()
                .ReverseMap();
            CreateMap<AddressDto, Address>()
                .ForMember(x => x.Customer, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<ContactInfo, ContactInfoDto>()
                .ForMember(ldto => ldto.ContactType, options => options.MapFrom(l => l.ContactInfoType))
                .ReverseMap();
            CreateMap<CustomerDto, Customer>().ReverseMap();
            CreateMap<CreateOrEditCustomerDto, Customer>().ReverseMap();
            CreateMap<CustomerInfoDto, Customer>()

                .ReverseMap();

            CreateMap<Customer, CustomerInfoDto>()
                .ForMember(x => x.CustomerType, opt => opt.Ignore())
                .ForMember(x => x.Spouse, opt => opt.Ignore())
                .ForMember(x => x.Dependent, opt => opt.Ignore()
                ).ReverseMap();
        }
    }
}
