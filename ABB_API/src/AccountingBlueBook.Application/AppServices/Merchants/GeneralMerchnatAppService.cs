using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using AccountingBlueBook.AppServices.paymentMethod.Dto;
using AccountingBlueBook.AppServices.Languages.Dto;
using AccountingBlueBook.AppServices.SourceReferralTypes;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingBlueBook.AppServices.entitytype.Dto;
using AccountingBlueBook.AppServices.Merchants.Dto;
using System.Linq.Dynamic.Core.Tokenizer;
using Abp.Authorization;

namespace AccountingBlueBook.AppServices.Merchants
{
    [AbpAuthorize]
    public  class MerchantAppService : AccountingBlueBookAppServiceBase, IGeneralMerchantAppService
    {
        private readonly IRepository<Merchant> _generalMerchantRepository;
        private IObjectMapper ObjectMapper;

        public MerchantAppService(IRepository<Merchant> merchantRepository, IObjectMapper objectMapper)
        {
            _generalMerchantRepository = merchantRepository;
            ObjectMapper = objectMapper;

        }
        public async Task CreateOrEdit(CreateOrEditMerchantInputDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Create(input);
        }
        [UnitOfWork]
        private async Task Create(CreateOrEditMerchantInputDto input)
          
                        {
            var  obj = new Merchant();
            obj.Name = input.Name;
            obj.APIKey=input.APIKey;
            obj.APISecretKey = input.APISecretKey;
            obj.Token = input.Token;
            obj.CCMID = input.CCMID;
            obj.Active = input.Active;
            obj.CCPWD = input.CCPWD;
            obj.CCUN=input.CCUN;
            await _generalMerchantRepository.InsertAsync(obj);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditMerchantInputDto input)
        {
            var generalMerchant = await _generalMerchantRepository.FirstOrDefaultAsync((int)input.Id);
            
            generalMerchant.Name = input.Name;
            generalMerchant.APIKey = input.APIKey;
            generalMerchant.APISecretKey = input.APISecretKey;

            generalMerchant.Token = input.Token;
            generalMerchant.CCMID = input.CCMID;
            generalMerchant.Active = input.Active;
            generalMerchant.CCUN = input.CCUN;
            generalMerchant.CCPWD = input.CCPWD;
            await _generalMerchantRepository.UpdateAsync(generalMerchant);
        }
        public async Task<GeneralMerchantDto> Get(EntityDto input)
        {
            var generalMerchant = await _generalMerchantRepository.GetAsync(input.Id);
            if (generalMerchant != null)
            {
                var obj = new GeneralMerchantDto();
                obj.Id = generalMerchant.Id;
                obj.Name = generalMerchant.Name;
                obj.Active = generalMerchant.Active;
                obj.APIKey = generalMerchant.APIKey;
                obj.CCUN = generalMerchant.CCUN;
                obj.CCPWD = generalMerchant.CCPWD;
                obj.APISecretKey = generalMerchant.APISecretKey;
                obj.Token = generalMerchant.Token;
                obj.CCMID = generalMerchant.CCMID;

                return obj;
            }
            return null;

        }

        public async Task<GeneralMerchantDto> CheckMerchantExistOrNot(string merchantName)
        {
            var haveMerchant =await _generalMerchantRepository.GetAll().Where(x => x.Name == merchantName).FirstOrDefaultAsync();
            if(haveMerchant!=null)
            {
                var obj = new GeneralMerchantDto();
                obj.Id = haveMerchant.Id;
                obj.Name = haveMerchant.Name;
                obj.Active = haveMerchant.Active;
                obj.APIKey = haveMerchant.APIKey;
                obj.CCUN = haveMerchant.CCUN;
                obj.CCPWD = haveMerchant.CCPWD;
                obj.APISecretKey = haveMerchant.APISecretKey;
                obj.Token = haveMerchant.Token;
                obj.CCMID = haveMerchant.CCMID;

                return obj;
            }
            return null;
        }
        public async Task<List<GeneralMerchantDto>> GetAll()
            {
            // todo :   CompanyId  
            var filteredQuery = _generalMerchantRepository.GetAll().AsQueryable();

            var data = from o in filteredQuery
                       select new GeneralMerchantDto
                       {
                           Id = o.Id,
                           Name = o.Name,
                           APIKey = o.APIKey,
                           APISecretKey = o.APISecretKey,
                           Token = o.Token,
                           CCMID=o.CCMID,
                           Active = o.Active,
                           CCUN=o.CCUN,
                           CCPWD=o.CCPWD,
                       
                       };
            return await data.ToListAsync();
        }
        public async Task Delete(EntityDto input)
        {
            var generalMerchantDto = await _generalMerchantRepository.FirstOrDefaultAsync((int)input.Id);
            await _generalMerchantRepository.DeleteAsync(generalMerchantDto);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

    }

}

