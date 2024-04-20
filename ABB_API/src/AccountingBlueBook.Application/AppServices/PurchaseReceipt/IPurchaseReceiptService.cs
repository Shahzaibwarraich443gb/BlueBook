using Abp.Application.Services;
using AccountingBlueBook.AppServices.PurchaseReceipt.Dto;
using AccountingBlueBook.AppServices.ReceivedPayment.Dto;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.PurchaseReceipt
{
    public interface IPurchaseReceiptService : IApplicationService
    {
        Task SavePurchaseReceipt(SavePurchaseReceiptDto input);
    }
}
