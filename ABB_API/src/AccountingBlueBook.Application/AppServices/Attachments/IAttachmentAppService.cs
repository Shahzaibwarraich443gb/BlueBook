using Abp.Application.Services.Dto;
using System.Threading.Tasks;

namespace AccountingBlueBook.AppServices.Attachments
{
    public interface IAttachmentAppService
    {
        Task SaveFile(EntityDto input);
    }
}
