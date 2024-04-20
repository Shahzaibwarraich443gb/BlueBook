using Abp.Application.Services;
using AccountingBlueBook.AppServices.CreditNote.Dto;
using AccountingBlueBook.AppServices.PurchaseReceipt.Dto;
using AccountingBlueBook.AppServices.ReceivedPayment.Dto;
using AccountingBlueBook.Entities.MainEntities.Customers;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.CreditNote
{
    public interface ICreditNoteService : IApplicationService
    {
        Task SaveCreditNote(SaveCreditNoteDto input);
    }
}
