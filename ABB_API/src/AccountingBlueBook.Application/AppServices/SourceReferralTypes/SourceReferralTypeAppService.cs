using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using AccountingBlueBook.AppServices.Adresses.Dto;
using AccountingBlueBook.AppServices.Emails.Dto;
using AccountingBlueBook.AppServices.Phones.Dto;
using AccountingBlueBook.AppServices.SourceReferralTypes.Dto;
using AccountingBlueBook.Entities.Main;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.SourceReferralTypes
{
    [AbpAuthorize]
    public class SourceReferralTypeAppService : AccountingBlueBookAppServiceBase, ISourceReferralTypeAppService
    {
        private readonly IRepository<SourceReferralType> _sourceReferralTypeRepository;
        private IObjectMapper ObjectMapper;

        public SourceReferralTypeAppService(IRepository<SourceReferralType> sourceReferralTypeRepository, IObjectMapper objectMapper)
        {
            _sourceReferralTypeRepository = sourceReferralTypeRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEdit(CreateOrEditSourceReferralTypeDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Cearte(input);
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditSourceReferralTypeDto input)
        {
            var sourceReferralType = await _sourceReferralTypeRepository.FirstOrDefaultAsync((int)input.Id);
             sourceReferralType.Name = input.Name;
            sourceReferralType.Code = input.Code; ;
            sourceReferralType.IsActive =  input.IsActive;
            sourceReferralType.Address = ObjectMapper.Map<Address>(input.Address);
            sourceReferralType.Phone = ObjectMapper.Map<Phone>(input.Phone);
            sourceReferralType.Email = ObjectMapper.Map<Email>(input.Email);
            //sourceReferralType.CompanyId = input.CompanyId;
            //sourceReferralType.Name = input.Name;
            //sourceReferralType.AddressId = input.AddressId;
            //sourceReferralType.Code = input.Code;
            //sourceReferralType.EmailId = input.EmailId;
            //sourceReferralType.PhoneId = input.PhoneId;

            await _sourceReferralTypeRepository.UpdateAsync(sourceReferralType);
        }

        [UnitOfWork]
        private async Task Cearte(CreateOrEditSourceReferralTypeDto input)
        {
            var sourceReferralType = ObjectMapper.Map<SourceReferralType>(input);
            sourceReferralType.Address = ObjectMapper.Map<Address>(input.Address);
            sourceReferralType.Phone = ObjectMapper.Map<Phone>(input.Phone);
            sourceReferralType.Email = ObjectMapper.Map<Email>(input.Email);
            await _sourceReferralTypeRepository.InsertAsync(sourceReferralType);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(EntityDto input)
        {
            var sourceReferralType = await _sourceReferralTypeRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            await _sourceReferralTypeRepository.DeleteAsync(sourceReferralType);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<SourceReferralTypeDto> Get(EntityDto input)
        {
            
            var sourceReferralType = await _sourceReferralTypeRepository.GetAll().Include(x=>x.Email).Include(x=>x.Phone).Include(x=>x.Address).FirstOrDefaultAsync(x => x.Id == input.Id);
            var data = ObjectMapper.Map<SourceReferralTypeDto>(sourceReferralType);
            data.CompanyId = (int?)sourceReferralType.CompanyId;
             data.Email = ObjectMapper.Map<CreateOrEditEmailInputDto>(sourceReferralType.Email);
            data.Address =  ObjectMapper.Map<CreateOrEditAddressDto>(sourceReferralType.Address);
            data.Phone = ObjectMapper.Map<CreateOrEditPhoneDto>(sourceReferralType.Phone); ;
        
            return data;
        }

        public async Task<List<SourceReferralTypeDto>> GetAll()
        {
            var filteredQuery = _sourceReferralTypeRepository.GetAll().Include(x => x.Address).AsQueryable();


            var data = from o in filteredQuery
                       select new SourceReferralTypeDto
                       {
                           Id = o.Id,
                           CompanyId = o.CompanyId,
                           Name = o.Name,
                           Code = o.Code,
                           EmailId = o.EmailId,
                           AddressId = o.AddressId,
                           PhoneId = o.PhoneId,
                           IsActive = o.IsActive,
                           Phone = ObjectMapper.Map<CreateOrEditPhoneDto>(o.Phone),
                           EmailAddress = o.Email == null ? null : o.Email.EmailAddress,
                           Company = o.Company == null ? null : o.Company.Name,
                           Address = ObjectMapper.Map<CreateOrEditAddressDto>(o.Address)
                       };
            return await data.ToListAsync();
        }
    }
}