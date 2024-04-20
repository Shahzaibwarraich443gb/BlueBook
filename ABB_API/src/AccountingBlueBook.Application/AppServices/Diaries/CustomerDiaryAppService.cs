using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using AccountingBlueBook.Entities.MainEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Diaries
{
    public class CustomerDiaryAppService : AccountingBlueBookAppServiceBase, ICustomerDiaryAppService
    {
        private readonly IRepository<CustomerDiary> _DiaryRepository;

        public CustomerDiaryAppService(IRepository<CustomerDiary> DiaryRepository)
        {
            _DiaryRepository = DiaryRepository;
        }
        public async Task SaveCustomerDiary(CustomerDiary input)
        {
            try
            {
                if (input.Id > 0)
                {
                    await _DiaryRepository.UpdateAsync(input);
                }                                                                                                                                                                                 
                else
                {
                    await _DiaryRepository.InsertAsync(input);
                }
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task DeleteCustomerDiary(EntityDto input)
        {
            try
            {
                await _DiaryRepository.DeleteAsync(input.Id);

                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<CustomerDiary>> CustomerDiaryGet(EntityDto input)
        {
            try
            {
                return await _DiaryRepository.GetAll().Where(x => x.CustomerId == input.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
