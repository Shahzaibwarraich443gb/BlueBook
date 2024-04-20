using System.Threading.Tasks;
using AccountingBlueBook.Models.TokenAuth;
using AccountingBlueBook.Web.Controllers;
using Shouldly;
using Xunit;

namespace AccountingBlueBook.Web.Tests.Controllers
{
    public class HomeController_Tests: AccountingBlueBookWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}