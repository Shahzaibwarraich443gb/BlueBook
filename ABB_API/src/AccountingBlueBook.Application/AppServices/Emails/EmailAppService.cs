using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using AccountingBlueBook.AppServices.Emails.Dto;
using AccountingBlueBook.AppServices.Emails;
using AccountingBlueBook.Entities.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.ObjectMapping;
using Microsoft.EntityFrameworkCore;

namespace AccountingBlueBook.AppServices.Emails
{
    public class EmailAppService : AccountingBlueBookAppServiceBase, IEmailAppService
    {
        private readonly IRepository<Email> _emailRepository;
        private IObjectMapper ObjectMapper;

        public EmailAppService(IRepository<Email> emailRepository, IObjectMapper objectMapper)
        {
            _emailRepository = emailRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEdit(CreateOrEditEmailInputDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Cearte(input);
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditEmailInputDto input)
        {
            var email = await _emailRepository.FirstOrDefaultAsync((int)input.Id);
            email.IsPrimary = input.IsPrimary;
            email.EmailAddress = input.EmailAddress;
            email.TypeEmail = input.TypeEmail;

            await _emailRepository.UpdateAsync(email);
        }

        [UnitOfWork]
        private async Task Cearte(CreateOrEditEmailInputDto input)
        {
            var email = ObjectMapper.Map<Email>(input);
            await _emailRepository.InsertAsync(email);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task Delete(EntityDto input)
        {
            var email = await _emailRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            await _emailRepository.DeleteAsync(email);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<EmailDto> Get(EntityDto input)
        {
            var email = await _emailRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            var data = ObjectMapper.Map<EmailDto>(email);
            return data;
        }

        public async Task<List<EmailDto>> GetAll()
        {
            var filteredQuery = _emailRepository.GetAll().AsQueryable();

            var data = from o in filteredQuery
                       select new EmailDto
                       {
                           Id = o.Id,
                           IsPrimary = o.IsPrimary,
                           EmailAddress = o.EmailAddress,
                           TypeEmail = o.TypeEmail,
                       };
            return await data.ToListAsync();
        }
    }
}