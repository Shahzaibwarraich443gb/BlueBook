using Abp.Application.Services.Dto;

namespace AccountingBlueBook.AppServices.Merchants.Dto
{
    public class CreateOrEditMerchantInputDto :  EntityDto
    {
        public string Name { get; set; }
        public string APIKey { get; set; }
        public string APISecretKey { get; set; }
        public string Token { get; set; }
        public string CCMID { get; set; }
        public string CCUN { get; set; }
        public string CCPWD { get; set; }
        public bool Active { get; set; }
    }
}