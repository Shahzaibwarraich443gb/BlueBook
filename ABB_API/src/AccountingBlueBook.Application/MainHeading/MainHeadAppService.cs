using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using AccountingBlueBook.AccountTypes.Dto;
using AccountingBlueBook.Entities;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.MainHeading.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.MainHeading
{
    public class MainHeadAppService : AccountingBlueBookAppServiceBase, IMainHeadAppService
    {
        private readonly IRepository<MainHead> _mainHeadRepository;
        private IObjectMapper ObjectMapper;

        public MainHeadAppService(IRepository<MainHead> mainHeadRepository, IObjectMapper objectMapper)
        {
            _mainHeadRepository = mainHeadRepository;
            ObjectMapper = objectMapper;
        }

        public async Task CreateOrEditMainHead(CreateOrEditMainHeadingInputDto input)
        {
                await CreateMainHead(input);
        }

        private async Task CreateMainHead(CreateOrEditMainHeadingInputDto input)
        {
            var maxCode = await _mainHeadRepository.GetAll().MaxAsync(x => x.Code);
            input.Code = (++maxCode).ToString();
            var mainHead = ObjectMapper.Map<MainHead>(input);
            await _mainHeadRepository.InsertAsync(mainHead);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task DeletMainHeads(EntityDto input)
        {
            var mainHead = await _mainHeadRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            await _mainHeadRepository.DeleteAsync(mainHead);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<MainHeadDto> GetMainHeads(EntityDto input)
        {
            var mainHead = await _mainHeadRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            return ObjectMapper.Map<MainHeadDto>(mainHead);
        }
    }
}
