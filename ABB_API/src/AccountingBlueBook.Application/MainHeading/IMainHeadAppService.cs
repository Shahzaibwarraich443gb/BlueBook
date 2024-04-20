using Abp.Application.Services.Dto;
using AccountingBlueBook.MainHeading.Dto;
using System.Threading.Tasks;

namespace AccountingBlueBook.MainHeading
{
    public interface IMainHeadAppService
    {
        Task CreateOrEditMainHead(CreateOrEditMainHeadingInputDto input);
        Task<MainHeadDto> GetMainHeads(EntityDto input);
        Task DeletMainHeads(EntityDto input);
    }
}
