using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.MainEntities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Diaries
{
    public interface ICustomerDiaryAppService
    {
        Task SaveCustomerDiary(CustomerDiary input);
        Task DeleteCustomerDiary(EntityDto input);
        Task<List<CustomerDiary>> CustomerDiaryGet(EntityDto input);
    }
}
