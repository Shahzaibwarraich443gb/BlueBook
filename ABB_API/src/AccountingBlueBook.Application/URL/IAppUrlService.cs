namespace AccountingBlueBook.URL
{
    public interface IAppUrlService
    {
        string CreateEmailActivationUrlFormat(string tenancyName);
    }
}
