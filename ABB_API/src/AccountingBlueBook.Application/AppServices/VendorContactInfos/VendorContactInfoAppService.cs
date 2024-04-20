using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using AccountingBlueBook.AppServices.ContactPersonTypes.Dto;
using AccountingBlueBook.AppServices.Languages;
using AccountingBlueBook.AppServices.Languages.Dto;
using AccountingBlueBook.AppServices.VendorContactInfos.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.VendorContactInfos
{
    public class VendorContactInfoAppService : AccountingBlueBookAppServiceBase, IVendorContactInfoAppService
    {
        private readonly IRepository<VendorContactInfo> _vendorContactInfoRepository;
        private readonly IRepository<VendorAddress> _vendorAddressesRepository;
        private IObjectMapper ObjectMapper;

        public VendorContactInfoAppService(IRepository<VendorContactInfo> vendorContactInfoRepository, IObjectMapper objectMapper,
           IRepository<VendorAddress> vendorAddressesRepository)
        {
            _vendorContactInfoRepository = vendorContactInfoRepository;
            ObjectMapper = objectMapper;
            _vendorAddressesRepository = vendorAddressesRepository;
        }

        public async Task<long> CreateOrEdit(CreateOrEditVendorContactInfoDto input)
        {
            long vendorId;

            if (input.Id > 0)
                vendorId = await Update(input);
            else
                vendorId = await Create(input);

            return vendorId;
        }

        [UnitOfWork]
        private async Task<long> Update(CreateOrEditVendorContactInfoDto input)
        {
            var obj = await _vendorContactInfoRepository.FirstOrDefaultAsync((int)input.Id);
            obj.ContactTypeId = input.ContactTypeId;
            obj.EmailTypeId = input.EmailTypeId;
            obj.WebSite = input.WebSite;
            obj.Fax = input.Fax;
            obj.EFax = input.EFax;
            obj.Primary = input.Primary;
            obj.PhoneNumber = input.PhoneNumber;
            obj.ContactPersonTypeId = input.ContactPersonTypeId;
            obj.ContactTypeName = input.ContactTypeName;
            obj.ContactPersonName = input.ContactPersonName;
            obj.EmailAddress = input.EmailAddress;
            obj.VendorId = input.VendorId;

            await _vendorContactInfoRepository.UpdateAsync(obj);
            return input.Id;
        }

        [UnitOfWork]
        private async Task<long> Create(CreateOrEditVendorContactInfoDto input)
        {
            var entityObj = new VendorContactInfo();
            entityObj.WebSite = input.WebSite;
            entityObj.Fax = input.Fax;
            entityObj.EmailTypeId = input.EmailTypeId;
            entityObj.ContactTypeId = entityObj.ContactTypeId;
            entityObj.EmailTypeId = input.EmailTypeId;
            entityObj.EFax = input.EFax;
            entityObj.VendorId = input.VendorId;
            entityObj.PhoneNumber = input.PhoneNumber;

            entityObj.Primary = input.Primary;
            entityObj.ContactPersonName = input.ContactPersonName;
            entityObj.ContactTypeName = input.ContactTypeName;
            entityObj.ContactPersonTypeId = input.ContactPersonTypeId;
            entityObj.EmailAddress = input.EmailAddress;
            long vendorId = await _vendorContactInfoRepository.InsertOrUpdateAndGetIdAsync(entityObj);
            await CurrentUnitOfWork.SaveChangesAsync();
            return vendorId;
        }

        public async Task Delete(EntityDto input)
        {

            var language = await _vendorContactInfoRepository.GetAll().FirstOrDefaultAsync(x => x.VendorId == input.Id);
            if (language != null)
            {
                await _vendorContactInfoRepository.DeleteAsync(language);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

        public async Task<VendorContactInfoDto> Get(EntityDto input)
        {
            var vendorContactInfo = await _vendorContactInfoRepository.GetAll().FirstOrDefaultAsync(x => x.VendorId == input.Id);
            if (vendorContactInfo != null)
            {
                var obj = new VendorContactInfoDto();
                obj.Id = vendorContactInfo.Id;
                obj.ContactTypeId = vendorContactInfo.ContactTypeId;
                obj.EmailTypeId = vendorContactInfo.EmailTypeId;
                obj.WebSite = vendorContactInfo.WebSite;
                obj.Fax = vendorContactInfo.Fax;
                obj.EFax = vendorContactInfo.EFax;
                obj.Primary = vendorContactInfo.Primary;
                obj.PhoneNumber = vendorContactInfo.PhoneNumber;
                obj.ContactPersonTypeId = vendorContactInfo.ContactPersonTypeId;
                obj.ContactTypeName = vendorContactInfo.ContactTypeName;
                obj.ContactPersonName = vendorContactInfo.ContactPersonName;
                obj.EmailAddress = vendorContactInfo.EmailAddress;
                obj.VendorId = vendorContactInfo.VendorId;

                return obj;
            }
            return null;

        }

        public async Task<List<VendorContactInfoDto>> GetAll()
        {
            //todo:  CompanyId 
            var filteredQuery = _vendorContactInfoRepository.GetAll().AsQueryable();


            var data = from o in filteredQuery
                       select new VendorContactInfoDto
                       {
                           Id = o.Id,
                           WebSite = o.WebSite,
                           Fax = o.Fax,
                           EFax = o.EFax,
                           Primary = o.Primary,
                           ContactPersonTypeId = o.ContactPersonTypeId,
                           ContactTypeName = o.ContactTypeName,
                           ContactPersonName = o.ContactPersonName,
                           EmailAddress = o.EmailAddress,
                       };
            return await data.ToListAsync();
        }

        ///////////////////////////////// ------------ Vendor Address ----------------- ///////////////////////////////////////////////

        public async Task<long> CreateOrEditAddress(CreateOrEditVendorAddressDto input)
        {
            long vendorId;
            if (input.Id > 0)
                vendorId = await UpdateAddress(input);
            else
                vendorId = await CreateAddress(input);
            return vendorId;
        }

        public async Task<long> CreateAddress(CreateOrEditVendorAddressDto input)
        {
            //var entityObj = new VendorAddressDto();
            var venderAddress = ObjectMapper.Map<VendorAddress>(input);
            long venderId = await _vendorAddressesRepository.InsertOrUpdateAndGetIdAsync(venderAddress);
            await CurrentUnitOfWork.SaveChangesAsync();
            return venderId;
        }
        public async Task<long> UpdateAddress(CreateOrEditVendorAddressDto input)
        {
            var venderAddress = ObjectMapper.Map<VendorAddress>(input);
            await _vendorAddressesRepository.UpdateAsync(venderAddress);
            await CurrentUnitOfWork.SaveChangesAsync();
            return input.Id;
        }
        public async Task<CreateOrEditVendorAddressDto> GetAddress(EntityDto input)
        {
            var venderAddress = await _vendorAddressesRepository.GetAll().FirstOrDefaultAsync(x => x.VendorId == input.Id);
            if (venderAddress != null)
            {
                var obj = new CreateOrEditVendorAddressDto();
                obj.Id = venderAddress.Id;
                obj.City = venderAddress.City;
                obj.CompleteAddress = venderAddress.CompleteAddress;
                obj.Country = venderAddress.Country;
                obj.VendorId = venderAddress.VendorId;
                obj.Fax = venderAddress.Fax;
                obj.IsPrimary = venderAddress.IsPrimary;
                obj.PostCode = venderAddress.PostCode;
                obj.State = venderAddress.State;
                obj.Type = venderAddress.Type;
                obj.VendorId = venderAddress.VendorId;
                return obj;
            }
            return null;
        }
        public async Task DeleteAddress(EntityDto input)
        {
            var del = await _vendorAddressesRepository.GetAll().FirstOrDefaultAsync(x => x.VendorId == input.Id); 
            if (del != null)
            {
                await _vendorAddressesRepository.DeleteAsync(del);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }
    }
}

