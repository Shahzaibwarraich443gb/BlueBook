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
using AccountingBlueBook.AppServices.PersonalInformations.Dto;
using AccountingBlueBook.AppServices.Invoices.dto;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using AccountingBlueBook.AppServices.ContactDetails;
using AccountingBlueBook.AppServices.ContactDetails.Dto;

namespace AccountingBlueBook.AppServices.CompanySettings
{
    [AbpAuthorize]
    public  class CompanySettingsAppService : AccountingBlueBookAppServiceBase, IGeneralCompanySettingsAppService
    {
        private readonly IRepository<Merchant> _generalMerchantRepository;
        private IObjectMapper ObjectMapper;

        public CompanySettingsAppService(IRepository<Merchant> merchantrespository, IObjectMapper objectMapper)
        {
            _generalMerchantRepository = merchantrespository;
            ObjectMapper = objectMapper;

        }

        public async Task<GeneralMerchantDto> GetMerchant()
        {
            var generalMerchant = await _generalMerchantRepository
                .GetAll()
                .Where(merchant => merchant.Active==true) // Filter where Active is true
                .FirstOrDefaultAsync();

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

        public async Task<GeneralMerchantDto> GetMerchantOnId(long Id)
        {
            var generalMerchant = await _generalMerchantRepository
                .GetAll()
                .Where(x => x.Id == Id).FirstOrDefaultAsync(); // Filter where Active is true
                

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


        public async Task<List<GeneralMerchantDto>> GetMerchants()
            {
            var merchants = await _generalMerchantRepository.GetAll().ToListAsync();
            if (merchants.Count() > 0)
            {
                var returnList = merchants.Select(x => new GeneralMerchantDto
                {
                    Name = x.Name,
                    Id = x.Id

                }).ToList();
                return returnList;
            }
            return null;

        }

        public async Task updateSelectedMerchant (long id)
        { 

            var previousSelectedMerchant = await _generalMerchantRepository
                .GetAll()
                .Where(x => x.Active==true).FirstOrDefaultAsync(); // Filter where Active is true
            if(previousSelectedMerchant!=null)
            {
                previousSelectedMerchant.Active = false;
                await _generalMerchantRepository.UpdateAsync(previousSelectedMerchant);
            }
            var merchant = await _generalMerchantRepository.GetAsync(Convert.ToInt16(id));
            if(merchant!=null)
            {
                merchant.Active = true;
                await _generalMerchantRepository.UpdateAsync(merchant);
            }

        }







    }

}

