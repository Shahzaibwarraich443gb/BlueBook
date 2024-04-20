using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using AccountingBlueBook.AppServices.Phones.Dto;
using AccountingBlueBook.AppServices.Phones;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.ObjectMapping;

namespace AccountingBlueBook.AppServices.Phones
{
    public class PhoneAppService : AccountingBlueBookAppServiceBase, IPhoneAppService
    {
        private readonly IRepository<Phone> _phoneRepository;
        private IObjectMapper ObjectMapper;

        public PhoneAppService(IRepository<Phone> phoneRepository, IObjectMapper objectMapper)
        {
            _phoneRepository = phoneRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEdit(CreateOrEditPhoneDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Cearte(input);
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditPhoneDto input)
        {
            var phone = await _phoneRepository.FirstOrDefaultAsync((int)input.Id);
            phone.IsPrimary = input.IsPrimary;
            phone.Number = input.Number;
            phone.Type = input.Type;

            await _phoneRepository.UpdateAsync(phone);
        }

        [UnitOfWork]
        private async Task Cearte(CreateOrEditPhoneDto input)
        {
            var phone = ObjectMapper.Map<Phone>(input);
            await _phoneRepository.InsertAsync(phone);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(EntityDto input)
        {
            var phone = await _phoneRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            await _phoneRepository.DeleteAsync(phone);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<PhoneDto> Get(EntityDto input)
        {
            var phone = await _phoneRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            var data = ObjectMapper.Map<PhoneDto>(phone);
            return data;
        }

        public async Task<List<PhoneDto>> GetAll()
        {
            var filteredQuery = _phoneRepository.GetAll().AsQueryable();

            var data = from o in filteredQuery
                       select new PhoneDto
                       {
                           Id = o.Id,
                           IsPrimary = o.IsPrimary,
                           Number = o.Number,
                           Type = o.Type,
                       };
            return await data.ToListAsync();
        }
    }
}