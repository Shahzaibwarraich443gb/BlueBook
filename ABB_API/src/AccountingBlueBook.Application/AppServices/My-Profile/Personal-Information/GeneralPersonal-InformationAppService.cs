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

namespace AccountingBlueBook.AppServices.PersonalInformations
{
    [AbpAuthorize]
    public  class PersonalInformationAppService : AccountingBlueBookAppServiceBase, IGeneralPersonalInformationAppService
    {
        private readonly IRepository<Employee,long> _generalPersonalInformationRepository;
        private IObjectMapper ObjectMapper;

        public PersonalInformationAppService(IRepository<Employee,long> personalInfoRepository, IObjectMapper objectMapper)
        {
            _generalPersonalInformationRepository = personalInfoRepository;
            ObjectMapper = objectMapper;

        }
        public async Task CreateOrEdit(CreateOrEditPersonalInformationInputDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Create(input);
        }
        [UnitOfWork]
        private async Task Create(CreateOrEditPersonalInformationInputDto input)
          
                        {
            var  obj = new Employee();
            obj.Title = input.Title;
            obj.FirstName=input.FirstName;
            obj.MiddleName = input.MiddleName;
            obj.LastName = input.LastName;
            obj.Suffix = input.Suffix;
            obj.Gender = input.Gender;
            obj.DateofBirth = input.DateofBirth;
            obj.EmployeeCode=input.EmployeeCode;
            obj.HireDate = input.HireDate;
            obj.DefaultSessionTimeout = input.DefaultSessionTimeout;
            await _generalPersonalInformationRepository.InsertAsync(obj);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditPersonalInformationInputDto input)
        {
            var generalpersonalInfo = await _generalPersonalInformationRepository.FirstOrDefaultAsync((int)input.Id);

            generalpersonalInfo.Title = input.Title;
            generalpersonalInfo.FirstName = input.FirstName;
            generalpersonalInfo.MiddleName = input.MiddleName;
            generalpersonalInfo.LastName = input.LastName;
            generalpersonalInfo.Suffix = input.Suffix;
            generalpersonalInfo.Gender = input.Gender;
            generalpersonalInfo.DateofBirth = input.DateofBirth;
            generalpersonalInfo.EmployeeCode = input.EmployeeCode;
            generalpersonalInfo .HireDate = input.HireDate;
            generalpersonalInfo.DefaultSessionTimeout = input.DefaultSessionTimeout;
            await _generalPersonalInformationRepository.UpdateAsync(generalpersonalInfo);
        }
        public async Task<GeneralPersonalInformationDto> Get()
        {
            var generalpersonalInfo = await _generalPersonalInformationRepository.GetAll().FirstOrDefaultAsync();
            if (generalpersonalInfo != null)
            {
               var result = ObjectMapper.Map<GeneralPersonalInformationDto>(generalpersonalInfo);
          


                return result;
            }
            return null;

        }


        //public async Task<List<GeneralPersonalInformationDto>> GetAll()
        //    {
        //    //// todo :   CompanyId  
        //    //var filteredQuery = _generalMerchantRepository.GetAll().AsQueryable();

        //    //var data = from o in filteredQuery
        //    //           select new GeneralMerchantDto
        //    //           {
        //    //               Id = o.Id,
        //    //               Name = o.Name,
        //    //               APIKey = o.APIKey,
        //    //               APISecretKey = o.APISecretKey,
        //    //               Token = o.Token,
        //    //               CCMID=o.CCMID,
        //    //               Active = o.Active,
        //    //               CCUN=o.CCUN,
        //    //               CCPWD=o.CCPWD,

        //    //           };
        //    //return await data.ToListAsync();
        //}
        public async Task Delete(EntityDto input)
        {
            var generalMerchantDto = await _generalPersonalInformationRepository.FirstOrDefaultAsync((int)input.Id);
            await _generalPersonalInformationRepository.DeleteAsync(generalMerchantDto);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

    }

}

