using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.Customers.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Customers
{
    public interface ICustomerAppService
    {
        Task<PagedResultDto<CustomerDto>> GetAll(PagedResultInputRequestDto input);
        Task<CreateOrEditCustomerDto> CreateOrEdit(CreateOrEditCustomerDto input);
        Task<CustomerDto> Get(EntityDto input);
        Task Delete(EntityDto input);
        Task<CreateOrEditCustomerDto> GetCustomerForEdit(EntityDto input);
        Task<CreateOrEditCustomerDto> UpdateCustomerDetail(CreateOrEditCustomerDto input);
        Task<CreateOrEditCustomerDto> UpdateCustomerContactInfo(CreateOrEditCustomerDto input);
        Task<CreateOrEditCustomerDto> UpdateCustomerAddress(CreateOrEditCustomerDto input);
        Task<CreateOrEditCustomerDto> UpdateCustomerUser(CreateOrEditCustomerDto input);
        Task<CreateOrEditCustomerDto> GetCustomerDetails(int customerId);
        Task<List<ContactInfoDto>> GetCustomerContact(int customerId);
        Task<List<UserNamePasswordDto>> GetCustomerUsers(int customerId);
        Task<List<CustomerAddressDto>> GetCustomerAddress(int customerId);
    }
}
