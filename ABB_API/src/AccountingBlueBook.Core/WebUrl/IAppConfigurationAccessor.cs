using Microsoft.Extensions.Configuration;

namespace AccountingBlueBook.WebUrl
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
