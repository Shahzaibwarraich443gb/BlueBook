using Abp.Application.Services;
using AccountingBlueBook.AppServices.ReceivedPayment.Dto;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.ReceivedPayment
{
    public interface IReceviedPaymentService : IApplicationService
    {
        Task<long> SaveReceivedPayment(SaveReceivedPayment input, int tenantId);
    }
}
