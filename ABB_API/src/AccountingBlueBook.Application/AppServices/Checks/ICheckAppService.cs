using AccountingBlueBook.Entities.MainEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Checks
{
    public interface ICheckAppService
    {
        Task<List<CheckDto>> GetCheck(bool showDeleted, int bankId);

        Task<Check> AddCheck(CheckDto input);

        Task<CheckDto> GetCheckById(CheckDto input);

        Task SaveCheckSetup(CheckSetupDto input);

        List<Dictionary<string, object>> GetCheckFooter();
    }
}
