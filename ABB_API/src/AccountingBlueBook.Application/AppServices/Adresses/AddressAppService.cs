using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using AccountingBlueBook.AppServices.Adresses.Dto;
using AccountingBlueBook.Entities.Main;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Adresses
{
    public class AddressAppService : AccountingBlueBookAppServiceBase, IAddressAppService
    {
        private readonly IRepository<Address> _addressRepository;
        private IObjectMapper ObjectMapper;

        public AddressAppService(IRepository<Address> addressRepository, IObjectMapper objectMapper)
        {
            _addressRepository = addressRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEdit(CreateOrEditAddressDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Cearte(input);
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditAddressDto input)
        {
            var address = await _addressRepository.FirstOrDefaultAsync((int)input.Id);
            address.PostCode = input.PostCode;
            address.City = input.City;
            address.State = input.State;
            address.CompleteAddress = input.CompleteAddress;
            address.Country = input.Country;
            address.Fax = input.Fax;
            address.IsPrimary = input.IsPrimary;

            await _addressRepository.UpdateAsync(address);
        }

        [UnitOfWork]
        private async Task Cearte(CreateOrEditAddressDto input)
        {
            var address = ObjectMapper.Map<Address>(input);
            await _addressRepository.InsertAsync(address);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(EntityDto input)
        {
            var address = await _addressRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            await _addressRepository.DeleteAsync(address);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<AddressDto> Get(EntityDto input)
        {
            var address = await _addressRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            var data = ObjectMapper.Map<AddressDto>(address);
            return data;
        }

        public async Task<List<AddressDto>> GetAll()
        {
            var filteredQuery = _addressRepository.GetAll().AsQueryable();

            var data = from o in filteredQuery
                       select new AddressDto
                       {
                           Id = o.Id,
                           City = o.City,
                           CompleteAddress= o.CompleteAddress,
                           State= o.State,
                           Country= o.Country,
                           IsPrimary= o.IsPrimary,
                           PostCode= o.PostCode,
                           Fax= o.Fax,
                           Type= o.Type
                       };
            return await data.ToListAsync();
        }
    }
}