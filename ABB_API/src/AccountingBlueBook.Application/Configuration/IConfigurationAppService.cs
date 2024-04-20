using System.Threading.Tasks;
using AccountingBlueBook.Configuration.Dto;

namespace AccountingBlueBook.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
