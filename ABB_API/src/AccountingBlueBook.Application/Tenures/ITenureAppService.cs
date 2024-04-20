using AccountingBlueBook.Entities.MainEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountingBlueBook.Tenures
{
    public interface ITenureAppService
    {
        List<TenureForm> GetAllTenureForms();
    }
}
