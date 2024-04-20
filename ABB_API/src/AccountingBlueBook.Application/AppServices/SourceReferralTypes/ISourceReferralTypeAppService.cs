using Abp.Application.Services.Dto;
using AccountingBlueBook.AppServices.SourceReferralTypes.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.SourceReferralTypes
{
    public interface ISourceReferralTypeAppService
    {
        Task<List<SourceReferralTypeDto>> GetAll();
        Task CreateOrEdit(CreateOrEditSourceReferralTypeDto input);
        Task<SourceReferralTypeDto> Get(EntityDto input);
        Task Delete(EntityDto input);
    }
}
