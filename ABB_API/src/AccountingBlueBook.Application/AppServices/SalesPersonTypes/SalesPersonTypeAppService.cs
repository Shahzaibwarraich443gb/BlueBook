using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using AccountingBlueBook.AppServices.SalesPersonTypes.Dto;
using AccountingBlueBook.AppServices.SalesPersonTypes;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.ObjectMapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AccountingBlueBook.AppServices.SalesPersonTypes
{
    [Authorize]
    public class SalesPersonTypeAppService : AccountingBlueBookAppServiceBase, ISalesPersonTypeAppService
    {
        private readonly IRepository<SalesPersonType> _salesPersonTypeRepository;
        private IObjectMapper ObjectMapper;

        public SalesPersonTypeAppService(IRepository<SalesPersonType> salesPersonTypeRepository, IObjectMapper objectMapper)
        {
            _salesPersonTypeRepository = salesPersonTypeRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEdit(CreateOrEditSalesPersonTypeDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Create(input);
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditSalesPersonTypeDto input)
        {
            var salesPersonType = await _salesPersonTypeRepository.FirstOrDefaultAsync((int)input.Id);
            salesPersonType.Phone = ObjectMapper.Map<Phone>(input.Phone);
            salesPersonType.Name = input.Name;
            salesPersonType.Code = input.Code;
            salesPersonType.IsActive = input.IsActive;
            ////salesPersonType.AddressId = input.AddressId;
            ////salesPersonType.PhoneId = input.PhoneId;

            await _salesPersonTypeRepository.UpdateAsync(salesPersonType);
        }

        [UnitOfWork]
        private async Task Create(CreateOrEditSalesPersonTypeDto input)
        {
            var salesPersonType = ObjectMapper.Map<SalesPersonType>(input);
            salesPersonType.CompleteAddress = input.Address.CompleteAddress;
            salesPersonType.City = input.Address.City;
            salesPersonType.State = input.Address.State;
            salesPersonType.Country = input.Address.Country;
            salesPersonType.PostCode = input.Address.PostCode;
            salesPersonType.Fax = input.Address.Fax;
            salesPersonType.IsPrimary = input.Address.IsPrimary;
            salesPersonType.Phone = ObjectMapper.Map<Phone>(input.Phone);
            await _salesPersonTypeRepository.InsertAsync(salesPersonType);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(EntityDto input)
        {
            var salesPersonType = await _salesPersonTypeRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            await _salesPersonTypeRepository.DeleteAsync(salesPersonType);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<CreateOrEditSalesPersonTypeDto> Get(EntityDto input)
        {
            var salesPersonType = await _salesPersonTypeRepository.GetAll().Include(data => data.Phone).FirstOrDefaultAsync(x => x.Id == input.Id);
            var data = ObjectMapper.Map<CreateOrEditSalesPersonTypeDto>(salesPersonType); 
            return data;
        }

        public async Task<List<SalesPersonTypeDto>> GetAll()
        {
            var filteredQuery = _salesPersonTypeRepository.GetAll().Include(data=>data.Phone).AsQueryable();
            //todo   Email;company

            var data = from o in filteredQuery
                       select new SalesPersonTypeDto
                       {
                           Id = o.Id,
                           CompanyId = o.CompanyId,
                           Name = o.Name, 
                           Code = o.Code,
                           IsActive = o.IsActive,
                           PhoneId = o.PhoneId,
                           Phone = o.Phone == null ? null : o.Phone,
                           //Company = o.Company == null ? null : o.Company,
                           PhoneNumber = o.Phone.Number ==null ? null : o.Phone.Number, 
                       };
            return await data.ToListAsync();
        }
    }
}