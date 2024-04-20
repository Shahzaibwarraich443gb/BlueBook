using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using AccountingBlueBook.AppServices.ContactPersonTypes.Dto;
using AccountingBlueBook.Entities.Main;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.ContactPersonTypes
{
    [AbpAuthorize]
    public class ContactPersonTypeAppService : AccountingBlueBookAppServiceBase, IContactPersonTypeAppService
    {
        private readonly IRepository<ContactPersonType> _contactPersonTypeRepository;
        private IObjectMapper ObjectMapper;

        public ContactPersonTypeAppService(IRepository<ContactPersonType> contactPersonTypeRepository, IObjectMapper objectMapper)
        {
            _contactPersonTypeRepository = contactPersonTypeRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEdit(CreateOrEditContactPersonTypeInputDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Cearte(input);
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditContactPersonTypeInputDto input)
        {
            var contactPersonType = await _contactPersonTypeRepository.FirstOrDefaultAsync((int)input.Id);
            contactPersonType.CompanyId = input.CompanyId;
           contactPersonType.IsActive  = input.IsActive;
            contactPersonType.Name = input.Name;
            await _contactPersonTypeRepository.UpdateAsync(contactPersonType);
        }

        [UnitOfWork]
        private async Task Cearte(CreateOrEditContactPersonTypeInputDto input)
        {
            var contactPersonType = ObjectMapper.Map<ContactPersonType>(input);
            contactPersonType.IsActive = true;
            await _contactPersonTypeRepository.InsertAsync(contactPersonType);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(EntityDto input)
        {
            var contactPersonType = await _contactPersonTypeRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            await _contactPersonTypeRepository.DeleteAsync(contactPersonType);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<ContactPersonTypeDto> Get(EntityDto input)
        {
            var contactPersonType = await _contactPersonTypeRepository.GetAll().Include(x=>x.Company).FirstOrDefaultAsync(x => x.Id == input.Id);
           if(contactPersonType!=null)
            {
               
                var obj = new ContactPersonTypeDto();
                obj.Id = contactPersonType.Id;
                obj.IsActive = contactPersonType.IsActive;
                obj.Name = contactPersonType.Name;
                obj.CompanyName = contactPersonType.Company!=null?contactPersonType.Company.Name:null;
                obj.CompanyId = contactPersonType.Company != null ? contactPersonType.Company.Id:null; 
                return obj;
            }
            return null;
        }

        public async Task<List<ContactPersonTypeDto>> GetAll()
        {
            var filteredQuery = _contactPersonTypeRepository.GetAll().Include(x=>x.Company).AsQueryable();

            var data = from o in filteredQuery
                       select new ContactPersonTypeDto
                       {
                           Id = o.Id,
                           IsActive = o.IsActive,   
                           CompanyId = o.CompanyId,
                           Name = o.Name,
                           CompanyName = o.Company == null ? null : o.Company.Name
                       };
            return await data.ToListAsync();
        }
    }
}
