using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Explorer.API.Controllers.Tourist.Payments;
using Explorer.API.Hubs;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class GiftCardCommandTests : BasePaymentsIntegrationTest
    {
        public GiftCardCommandTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public async Task PurchaseGiftCard_returns_bad_request_when_recipient_username_empty()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateGiftCardController(scope, "-1");

            var result = await controller.PurchaseGiftCard(new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "",
                Amount = 10m,
                PaymentMethod = "CreditCard",
                CardNumber = "1234567890123456",
                CardHolderName = "Test",
                ExpiryDate = "12/28",
                CVV = "123"
            });

            result.Result.ShouldBeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)result.Result).Value?.ToString().ShouldContain("Recipient username");
        }

        [Fact]
        public async Task PurchaseGiftCard_returns_bad_request_when_amount_zero()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateGiftCardController(scope, "-1");

            var result = await controller.PurchaseGiftCard(new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "someone",
                Amount = 0m,
                PaymentMethod = "CreditCard",
                CardNumber = "1234567890123456",
                CardHolderName = "Test",
                ExpiryDate = "12/28",
                CVV = "123"
            });

            result.Result.ShouldBeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)result.Result).Value?.ToString().ShouldContain("Amount");
        }

        [Fact]
        public async Task PurchaseGiftCard_returns_bad_request_when_recipient_not_found()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateGiftCardController(scope, "-1");

            var result = await controller.PurchaseGiftCard(new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "nonexistent_user_xyz_123",
                Amount = 10m,
                PaymentMethod = "CreditCard",
                CardNumber = "1234567890123456",
                CardHolderName = "Test",
                ExpiryDate = "12/28",
                CVV = "123"
            });

            result.Result.ShouldBeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)result.Result).Value?.ToString().ShouldContain("not found");
        }

        [Fact]
        public async Task PurchaseGiftCard_returns_bad_request_when_invalid_payment_method()
        {
            using var scope = Factory.Services.CreateScope();
            var stakeholdersDb = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            EnsureGiftRecipientUserExists(stakeholdersDb);
            var controller = CreateGiftCardController(scope, "-1");

            var result = await controller.PurchaseGiftCard(new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "gift_recipient",
                Amount = 10m,
                PaymentMethod = "GiftCard"
            });

            result.Result.ShouldBeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)result.Result).Value?.ToString().ShouldContain("Invalid payment method");
        }

        [Fact]
        public async Task PurchaseGiftCard_returns_bad_request_when_credit_card_invalid()
        {
            using var scope = Factory.Services.CreateScope();
            var stakeholdersDb = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            EnsureGiftRecipientUserExists(stakeholdersDb);
            var controller = CreateGiftCardController(scope, "-1");

            var result = await controller.PurchaseGiftCard(new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "gift_recipient",
                Amount = 10m,
                PaymentMethod = "CreditCard",
                CardNumber = "123"
            });

            result.Result.ShouldBeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)result.Result).Value?.ToString().ShouldContain("card", Case.Insensitive);
        }

        [Fact]
        public async Task PurchaseGiftCard_returns_bad_request_when_paypal_invalid()
        {
            using var scope = Factory.Services.CreateScope();
            var stakeholdersDb = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            EnsureGiftRecipientUserExists(stakeholdersDb);
            var controller = CreateGiftCardController(scope, "-1");

            var result = await controller.PurchaseGiftCard(new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "gift_recipient",
                Amount = 10m,
                PaymentMethod = "PayPal",
                PayPalEmail = "not-an-email"
            });

            result.Result.ShouldBeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)result.Result).Value?.ToString().ShouldContain("PayPal", Case.Insensitive);
        }

        [Fact]
        public async Task PurchaseGiftCard_returns_ok_when_valid_credit_card_and_recipient_exists()
        {
            using var scope = Factory.Services.CreateScope();
            var stakeholdersDb = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var paymentsDb = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            EnsureGiftRecipientUserExists(stakeholdersDb);

            var controller = CreateGiftCardController(scope, "-1");

            var result = await controller.PurchaseGiftCard(new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "gift_recipient",
                Amount = 25m,
                PaymentMethod = "CreditCard",
                CardNumber = "1234567890123456",
                CardHolderName = "Buyer",
                ExpiryDate = "12/28",
                CVV = "456"
            });

            result.Result.ShouldBeOfType<OkObjectResult>();
            var dto = ((OkObjectResult)result.Result).Value as GiftCardDto;
            dto.ShouldNotBeNull();
            dto.Amount.ShouldBe(25m);
            dto.Balance.ShouldBe(25m);
            dto.Code.Length.ShouldBeGreaterThanOrEqualTo(10);
            dto.RecipientTouristId.ShouldNotBeNull();

            paymentsDb.GiftCards.Any(g => g.Code == dto.Code && g.BuyerTouristId == -1).ShouldBeTrue();
        }

        [Fact]
        public async Task PurchaseGiftCard_returns_ok_when_valid_paypal_and_recipient_exists()
        {
            using var scope = Factory.Services.CreateScope();
            var stakeholdersDb = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var paymentsDb = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            EnsureGiftRecipientUserExists(stakeholdersDb);

            var controller = CreateGiftCardController(scope, "-1");

            var result = await controller.PurchaseGiftCard(new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "gift_recipient",
                Amount = 15m,
                PaymentMethod = "PayPal",
                PayPalEmail = "buyer@example.com"
            });

            result.Result.ShouldBeOfType<OkObjectResult>();
            var dto = ((OkObjectResult)result.Result).Value as GiftCardDto;
            dto.ShouldNotBeNull();
            dto.Amount.ShouldBe(15m);
            dto.Balance.ShouldBe(15m);

            paymentsDb.GiftCards.Count(g => g.BuyerTouristId == -1).ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task PurchaseGiftCard_returns_bad_request_when_buy_for_self()
        {
            using var scope = Factory.Services.CreateScope();
            var stakeholdersDb = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            EnsureGiftRecipientUserExists(stakeholdersDb);

            var personId = stakeholdersDb.People.First(p => p.UserId == -10).Id;

            var controller = CreateGiftCardController(scope, personId.ToString());

            var result = await controller.PurchaseGiftCard(new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "gift_recipient",
                Amount = 10m,
                PaymentMethod = "CreditCard",
                CardNumber = "1234567890123456",
                CardHolderName = "Test",
                ExpiryDate = "12/28",
                CVV = "123"
            });

            result.Result.ShouldBeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)result.Result).Value?.ToString().ShouldContain("yourself");
        }

        private static void EnsureGiftRecipientUserExists(StakeholdersContext db)
        {
            if (db.Users.Any(u => u.Id == -10)) return;

            var user = new Explorer.Stakeholders.Core.Domain.User(
                "gift_recipient",
                "password",
                Explorer.Stakeholders.Core.Domain.UserRole.Tourist,
                true);
            db.Users.Add(user);
            db.Entry(user).Property("Id").CurrentValue = -10L;
            db.SaveChanges();

            var person = new Explorer.Stakeholders.Core.Domain.Person(-10L, "Gift", "Recipient", "gift_recipient@example.com");
            db.People.Add(person);
            db.SaveChanges();
        }

        private static GiftCardController CreateGiftCardController(IServiceScope scope, string touristId)
        {
            return new GiftCardController(
                scope.ServiceProvider.GetRequiredService<IGiftCardService>(),
                scope.ServiceProvider.GetRequiredService<INotificationService>(),
                scope.ServiceProvider.GetRequiredService<IHubContext<MessageHub>>())
            {
                ControllerContext = BuildTouristContext(touristId)
            };
        }
    }
}
