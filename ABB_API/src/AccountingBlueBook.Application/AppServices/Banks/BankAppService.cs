using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using AccountingBlueBook.AppServices.Adresses.Dto;
using AccountingBlueBook.AppServices.Banks.Dto;
using AccountingBlueBook.AppServices.Emails.Dto;
using AccountingBlueBook.AppServices.Phones.Dto;
using AccountingBlueBook.AppServices.SourceReferralTypes;
using AccountingBlueBook.AppServices.SourceReferralTypes.Dto;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Banks
{
    [AbpAuthorize]
    public  class BankAppService : AccountingBlueBookAppServiceBase, IBankAppService
    {
        private readonly IRepository<Bank> _bankRepository;
        private IObjectMapper ObjectMapper;

        public BankAppService(IRepository<Bank> bankRepository, IObjectMapper objectMapper)
        {
            _bankRepository = bankRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEdit(CreateOrEditBankDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Cearte(input);
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditBankDto input)
        {
            var bank = await _bankRepository.FirstOrDefaultAsync((int)input.Id);
            bank.StartingCheque = input.StartingCheque;
            bank.SwiftCode = input.SwiftCode;
            bank.Routing=input.Routing;
            bank.BankName = input.BankName;
            bank.TitleofAccount= input.TitleofAccount; 
            bank.AccountNumber = input.AccountNumber; 
            bank.IsActive = input.IsActive;
            bank.BankAddress = ObjectMapper.Map<BankAddress>(input.Address);  
            await _bankRepository.UpdateAsync(bank);
        }

        [UnitOfWork]
        private async Task Cearte(CreateOrEditBankDto input)
        {
            var bank = ObjectMapper.Map<Bank>(input);
            bank.IsActive = true;
            bank.BankAddress = ObjectMapper.Map<BankAddress>(input.Address); 
            await _bankRepository.InsertAsync(bank);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(EntityDto input)
        {
            var bank = await _bankRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            await _bankRepository.DeleteAsync(bank);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<BankDto> Get(EntityDto input)
        {

            var bank = await _bankRepository.GetAll() .Include(x => x.BankAddress).FirstOrDefaultAsync(x => x.Id == input.Id);
            var data = ObjectMapper.Map<BankDto>(bank);  
            data.Address = ObjectMapper.Map<CreateOrEditBankAddressDto>(bank.BankAddress);  
            return data;
        }

        public async Task<List<BankDto>> GetAll()
        {
            var filteredQuery = await _bankRepository.GetAll().AsQueryable().ToListAsync();

            var data = from o in filteredQuery
                       select new BankDto
                       {
                           Id = o.Id, 
                           BankName = o.BankName,
                           TitleofAccount = o.TitleofAccount,
                           AccountNumber = o.AccountNumber,
                           Routing = o.Routing,
                           IsActive = o.IsActive,
                           OpenBalance = o.OpenBalance
                          
                       };
            return data.ToList();
        }
    }
}
