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
using AccountingBlueBook.Authorization.Users;
using AccountingBlueBook.Users.Dto;

namespace AccountingBlueBook.AppServices.ContactDetails
{
    [AbpAuthorize]
    public  class ContactDetailsAppService : AccountingBlueBookAppServiceBase, IGeneralContactDetailsAppService
    {
        private readonly IRepository<Employee,long> _generalContactDetailsRepository;
        private readonly IRepository<User, long> _generuserrepository;
        private IObjectMapper ObjectMapper;

        public ContactDetailsAppService(IRepository<Employee,long> ContactDetailsRepository, IRepository<User, long> generuserrepository, IObjectMapper objectMapper)
        {
            _generalContactDetailsRepository = ContactDetailsRepository;
            _generuserrepository = generuserrepository;
            ObjectMapper = objectMapper;

        }
        public async Task CreateOrEdit(CreateOrEditContactDetalsInputDto input)
        {
            if (input.Id > 0)
                await Update(input);
            else
                await Create(input);
        }
        [UnitOfWork]
        private async Task Create(CreateOrEditContactDetalsInputDto input)
          
               {
            var  obj = new Employee();
            obj.Email = input.Email;
            obj.PhoneNumber=input.PhoneNumber;
            obj.MobileNumber = input.MobileNumber;
            obj.Address = input.Address;
            obj.City = input.City;
            obj.State = input.State;
            obj.Country = input.Country;
            obj.ZipCode = input.PostCode;
            
            
            await _generalContactDetailsRepository.InsertAsync(obj);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditContactDetalsInputDto input)
        {
            var generalContactdetails = await _generalContactDetailsRepository.FirstOrDefaultAsync((int)input.Id);

            generalContactdetails.Email = input.Email;
            generalContactdetails.PhoneNumber = input.PhoneNumber;
            generalContactdetails.MobileNumber = input.MobileNumber;
            generalContactdetails.Address = input.Address;
            generalContactdetails.City = input.City;
            generalContactdetails.State = input.State;
            generalContactdetails.Country = input.Country;
            generalContactdetails.ZipCode = input.PostCode;
            
           
            await _generalContactDetailsRepository.UpdateAsync(generalContactdetails);
        }
        public async Task<GeneralContactDetailsDto> Get()
        {
            var contactDetails = await _generalContactDetailsRepository.GetAll().FirstOrDefaultAsync();
            if (contactDetails != null)
            {
               var result = ObjectMapper.Map<GeneralContactDetailsDto>(contactDetails);
          


                return result;
            }
            return null;

        }

        public async Task<User> GetAlluser()
        {
            var users = _generuserrepository.GetAll().FirstOrDefaultAsync();
            return await users;




        }
        public async Task<UserDto> GetAdminUser()
        {
            var users =await _generuserrepository.GetAll().Where(x => x.TenantId == AbpSession.TenantId).Select(x => new UserDto
            {

                EmailAddress = x.EmailAddress,
                UserName = x.UserName
            }).FirstOrDefaultAsync();
            return users;

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
            var contactdetails = await _generalContactDetailsRepository.FirstOrDefaultAsync((int)input.Id);
            await _generalContactDetailsRepository.DeleteAsync(contactdetails);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

    }

}

