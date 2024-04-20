using AccountingBlueBook.AppServices.Venders.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.VendorContactInfos.Dto
{
    public class VendorContactInfoProfile : Profile
    {
        public VendorContactInfoProfile()
        {
            CreateMap<VendorContactInfoDto, VendorContactInfo>().ReverseMap();
            CreateMap<CreateOrEditVenderDto, VendorContactInfo>().ReverseMap();
            CreateMap<VendorAddressDto, VendorAddress>().ReverseMap();
            CreateMap<CreateOrEditVendorAddressDto, VendorAddress>().ReverseMap();
        }
    }
}
