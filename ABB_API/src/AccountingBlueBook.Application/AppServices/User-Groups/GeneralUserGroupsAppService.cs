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
using AccountingBlueBook.AppServices.usersGroup.Dto;

namespace AccountingBlueBook.AppServices.usersGroup
{
    public  class UserGroupsAppService : AccountingBlueBookAppServiceBase, IGeneralUsersGroupsAppService
    {
        private readonly IRepository<UsersGroup> _generalUsersGroupRepository;
        private IObjectMapper ObjectMapper;

        public UserGroupsAppService(IRepository<UsersGroup> usergrouprepository, IObjectMapper objectMapper)
        {
            _generalUsersGroupRepository = usergrouprepository;
            ObjectMapper = objectMapper;

        }
        public async Task CreateOrEdit(CreateOrEditGeneralUserGroupInputDto input)
                    {
            if (input.Id > 0)
                await Update(input);
            else
                await Cearte(input);
        }
        [UnitOfWork]
        private async Task Cearte(CreateOrEditGeneralUserGroupInputDto input)
                        {
            var createOrEditUserGroup = ObjectMapper.Map<UsersGroup>(input);
             createOrEditUserGroup.TenantId = Convert.ToInt32(AbpSession.TenantId);
           
            createOrEditUserGroup.IsActive = true;
            createOrEditUserGroup.TenantId = AbpSession.TenantId == null ? 1 : (int)AbpSession.TenantId;
            await _generalUsersGroupRepository.InsertAsync(createOrEditUserGroup);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        [UnitOfWork]
        private async Task Update(CreateOrEditGeneralUserGroupInputDto input)
        {
            var generalusergroup = await _generalUsersGroupRepository.FirstOrDefaultAsync((int)input.Id);
            //language.CompanyId = input.CompanyId;
            generalusergroup.Name = input.Name;
            generalusergroup.IpRestrictionUserGroup = input.IpRestrictionUserGroup;
            generalusergroup.IpAddress = input.IpAddress;
            generalusergroup.IsActive = input.IsActive;
            await _generalUsersGroupRepository.UpdateAsync(generalusergroup);
        }




        public async Task<GeneralUsersGroupDto> Get(EntityDto input)
        {
            var generalusergroup = await _generalUsersGroupRepository.GetAll().FirstOrDefaultAsync(x => x.Id == input.Id);
            if (generalusergroup != null)
            {
                var obj = new GeneralUsersGroupDto();
                obj.Id = generalusergroup.Id;
                obj.Name = generalusergroup.Name;
                obj.IpAddress = generalusergroup.IpAddress;
                obj.IpRestrictionUserGroup = generalusergroup.IpRestrictionUserGroup;
                obj.IsActive = generalusergroup.IsActive;
                return obj;
            }
            return null;

        }

        public async Task<List<GeneralUsersGroupDto>> GetAll()
            {
            // todo :   CompanyId  
            var filteredQuery = _generalUsersGroupRepository.GetAll().AsQueryable();

            var data = from o in filteredQuery
                       select new GeneralUsersGroupDto
                       {
                           Id = o.Id,
                           IsActive = o.IsActive,
                           //CompanyId = o.CompanyId,
                           Name = o.Name,
                           IpAddress = o.IpAddress,
                           IpRestrictionUserGroup=o.IpRestrictionUserGroup

                       };
            return await data.ToListAsync();
        }


        public async Task Delete(EntityDto input)
        {
            var generalpaymentmethod = await _generalUsersGroupRepository.FirstOrDefaultAsync((int)input.Id);
            await _generalUsersGroupRepository.DeleteAsync(generalpaymentmethod);
            await CurrentUnitOfWork.SaveChangesAsync();
        }




    }


}

