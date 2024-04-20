using AccountingBlueBook.Entities.MainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.LegalStatuses
{
    public interface ILegalStatusAppService
    {
        List<LegalStatus> GetAllLegalStatus();
    }
}
