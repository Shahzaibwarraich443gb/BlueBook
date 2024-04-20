using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using AccountingBlueBook.AccountTypes.Dto;
using AccountingBlueBook.Entities.MainEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingBlueBook.AccountTypes
{
    public class AccountTypeAppService : AccountingBlueBookAppServiceBase, IAccountTypeAppService
    {
        private readonly IRepository<AccountType> _accountTypeRepository;
        private IObjectMapper ObjectMapper;

        public AccountTypeAppService(
            IRepository<AccountType> accountTypeRepository,
            IObjectMapper objectMapper
            )
        {
            _accountTypeRepository = accountTypeRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEditAccountType(CreateOrEditAccountTypeInputDto input)
        {
            await CreateAccountType(input);
        }

        private async Task CreateAccountType(CreateOrEditAccountTypeInputDto input)
        {
            var maxCode = await _accountTypeRepository.GetAll().MaxAsync(x => x.Code);
            input.Code = (++maxCode).ToString();
            var accountType = ObjectMapper.Map<AccountType>(input);
            await _accountTypeRepository.InsertAsync(accountType);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task DeletAccountTypes(EntityDto input)
        {
            var accountType = await _accountTypeRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            await _accountTypeRepository.DeleteAsync(accountType);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<AccountTypeDto> GetAccountTypes(EntityDto input)
        {
            var accountType = await _accountTypeRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            return ObjectMapper.Map<AccountTypeDto>(accountType);
        }
    }
}
