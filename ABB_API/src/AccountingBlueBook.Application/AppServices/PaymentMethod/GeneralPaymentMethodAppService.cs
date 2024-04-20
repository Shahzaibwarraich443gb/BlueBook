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

namespace AccountingBlueBook.AppServices.paymentMethod
{
    public  class PaymentMethodAppService : AccountingBlueBookAppServiceBase, IGeneralPaymentMethodeAppService
    {
        private readonly IRepository<PaymentMethod> _generalPayementMethodRepository;
        private IObjectMapper ObjectMapper;

        public PaymentMethodAppService(IRepository<PaymentMethod> paymentmethodrepository, IObjectMapper objectMapper)
        {
            _generalPayementMethodRepository = paymentmethodrepository;
            ObjectMapper = objectMapper;

        }
        public async Task CreateOrEdit(CreateOrEditGeneralPaymentMethodInputDto input)
                    {
            if (input.Id > 0)
                await Update(input);
            else
                await Cearte(input);
        }
        [UnitOfWork]
        private async Task Cearte(CreateOrEditGeneralPaymentMethodInputDto input)
                        {
            var createOrEditpaymentmethod = ObjectMapper.Map<PaymentMethod>(input);
             createOrEditpaymentmethod.TenantId = Convert.ToInt32(AbpSession.TenantId);
           
            createOrEditpaymentmethod.IsActive = true;
            createOrEditpaymentmethod.TenantId = AbpSession.TenantId == null ? 1 : (int)AbpSession.TenantId;
            await _generalPayementMethodRepository.InsertAsync(createOrEditpaymentmethod);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditGeneralPaymentMethodInputDto input)
        {
            var generalpaymentmethod = await _generalPayementMethodRepository.FirstOrDefaultAsync((int)input.Id);
            //language.CompanyId = input.CompanyId;
            generalpaymentmethod.Name = input.Name;
            generalpaymentmethod.IsActive = input.IsActive;
            await _generalPayementMethodRepository.UpdateAsync(generalpaymentmethod);
        }




        public async Task<GeneralPaymentMethodDto> Get(EntityDto input)
        {
            var generalpayementmethod = await _generalPayementMethodRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            if (generalpayementmethod != null)
            {
                var obj = new GeneralPaymentMethodDto();
                obj.Id = generalpayementmethod.Id;
                obj.Name = generalpayementmethod.Name;
                obj.IsActive = generalpayementmethod.IsActive;
                return obj;
            }
            return null;

        }

        public async Task<List<GeneralPaymentMethodDto>> GetAll()
            {
            // todo :   CompanyId  
            var filteredQuery = _generalPayementMethodRepository.GetAll().AsQueryable();

            var data = from o in filteredQuery
                       select new GeneralPaymentMethodDto
                       {
                           Id = o.Id,
                           IsActive = o.IsActive,
                           //CompanyId = o.CompanyId,
                           Name = o.Name,

                       };
            return await data.ToListAsync();
        }


        public async Task Delete(EntityDto input)
        {
            var generalpaymentmethod = await _generalPayementMethodRepository.FirstOrDefaultAsync((int)input.Id);
            await _generalPayementMethodRepository.DeleteAsync(generalpaymentmethod);
            await CurrentUnitOfWork.SaveChangesAsync();
        }




    }


}

