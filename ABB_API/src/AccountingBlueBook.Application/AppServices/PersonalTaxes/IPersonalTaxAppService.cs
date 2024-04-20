using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities.MainEntities;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.PersonalTaxes
{
    public interface IPersonalTaxAppService
    {
        Task SavePersonalTax(PersonalTaxDto input);
        Task<PersonalTaxDto> PersonalTaxGet(PersonalTaxDto input);
    }
}
