using Abp.Application.Services.Dto;

namespace AccountingBlueBook.AppServices.paymentMethod.Dto
{
    public class CreateOrEditGeneralPaymentMethodInputDto :  EntityDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}