namespace AccountingBlueBook.WebUrl
{
    public interface IWebUrlService
    {
        string WebSiteRootAddressFormat { get; }

        string ServerRootAddressFormat { get; }

        string GetSiteRootAddress(string tenancyName = null);

        string GetServerRootAddress(string tenancyName = null);

    }
}
