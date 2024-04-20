using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using AccountingBlueBook.AppServices.CustomerTypes.Dto;
using AccountingBlueBook.AppServices.EntityTypes;
using AccountingBlueBook.AppServices.EntityTypes.Dto;
using AccountingBlueBook.Entities.MainEntities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.CustomerTypes
{
    public  class CustomerTypesAppService : AccountingBlueBookAppServiceBase, ICustomerTypesAppService
    {
        private readonly IRepository<CustomerType> _customerTypeRepository;
        private IObjectMapper ObjectMapper;

        public CustomerTypesAppService(IRepository<CustomerType> customerTypeRepository, IObjectMapper objectMapper)
        {
            _customerTypeRepository = customerTypeRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEdit(CreateOrEditCustomerTypeInputDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Cearte(input);
        }
        [UnitOfWork]
        private async Task Update(CreateOrEditCustomerTypeInputDto input)
        {
            var  customerType = await _customerTypeRepository.FirstOrDefaultAsync((int)input.Id);
            customerType.Name = input.Name;
            customerType.IsActive = input.IsActive;
            await _customerTypeRepository.UpdateAsync(customerType);
        }

        [UnitOfWork]
        private async Task Cearte(CreateOrEditCustomerTypeInputDto input)
        {
            var customerType = ObjectMapper.Map<CustomerType>(input);
             customerType.IsActive = true;
            await _customerTypeRepository.InsertAsync(customerType);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(EntityDto input)
        {
            var customerType = await _customerTypeRepository.FirstOrDefaultAsync((int)input.Id);
            await _customerTypeRepository.DeleteAsync(customerType);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task<CustomerTypeDto> Get(EntityDto input)
        {
            var customerType = await _customerTypeRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            if (customerType != null)
            {
                var obj = new CustomerTypeDto();
                obj.Id = customerType.Id;
                obj.Name = customerType.Name;
                obj.IsActive = customerType.IsActive;
                return obj;
            }
            return null;

        }

        public async Task<List<CustomerTypeDto>> GetAll()
        { 
            var filteredQuery = _customerTypeRepository.GetAll().AsQueryable();

            var data = from o in filteredQuery
                       select new CustomerTypeDto
                       {
                           Id = o.Id,
                           //IsActive = o.IsActive, 
                           Name = o.Name,

                       };
            return await data.ToListAsync();
        }
    }
}
