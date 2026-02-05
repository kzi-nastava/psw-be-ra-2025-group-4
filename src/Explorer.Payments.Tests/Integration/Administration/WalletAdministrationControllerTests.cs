using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Payments.API.Public.Administration;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class WalletAdministrationControllerTests : BasePaymentsIntegrationTest
    {
        public WalletAdministrationControllerTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void AddBalance_increases_wallet_balance()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();

            var touristId = -21;
            var before = walletService.GetWallet(touristId).Balance;

            var result = controller.AddBalance(touristId, new AddBalanceDto { Amount = 50 }).Result;
            result.ShouldBeOfType<OkResult>();

            var after = walletService.GetWallet(touristId).Balance;
            after.ShouldBe(before + 50);
        }

        [Fact]
        public void AddBalance_returns_bad_request_for_negative_amount()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = controller.AddBalance(-21, new AddBalanceDto { Amount = -10 }).Result;
            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        private static WalletAdministrationController CreateController(IServiceScope scope)
        {
            return new WalletAdministrationController(
                scope.ServiceProvider.GetRequiredService<IWalletAdministrationService>(),
                scope.ServiceProvider.GetRequiredService<INotificationService>(),
                scope.ServiceProvider.GetRequiredService<IHubContext<Explorer.API.Hubs.MessageHub>>());
        }
    }
}
