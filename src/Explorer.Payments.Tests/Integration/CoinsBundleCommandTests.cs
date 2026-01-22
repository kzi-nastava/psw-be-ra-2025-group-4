using System;
using System.Linq;
using Explorer.API.Controllers.Tourist.Payments;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class CoinsBundleCommandTests : BasePaymentsIntegrationTest
    {
        public CoinsBundleCommandTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Purchases_bundle_with_credit_card_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var walletBefore = db.Wallets.First(w => w.TouristId == -1);
            var balanceBefore = walletBefore.Balance;

            var request = new PurchaseCoinsBundleRequestDto
            {
                CoinsBundleId = 1,
                PaymentMethod = "CreditCard",
                CardNumber = "1234567890123456",
                CardHolderName = "Test User",
                ExpiryDate = "12/25",
                CVV = "123"
            };

            var result = PurchaseAndGetDto(controller, request);

            result.TouristId.ShouldBe(-1);
            result.CoinsBundleId.ShouldBe(1);
            result.BundleName.ShouldBe("Starter Pack");
            result.CoinsReceived.ShouldBe(500);
            result.PricePaid.ShouldBe(5.00m);
            result.PaymentMethod.ShouldBe("CreditCard");
            result.TransactionId.ShouldStartWith("CC-");

            var walletAfter = db.Wallets.First(w => w.TouristId == -1);
            walletAfter.Balance.ShouldBe(balanceBefore + 500);

            db.CoinsBundlePurchases.Any(p => p.Id == result.Id).ShouldBeTrue();
        }

        [Fact]
        public void Purchases_bundle_with_paypal_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var request = new PurchaseCoinsBundleRequestDto
            {
                CoinsBundleId = 2,
                PaymentMethod = "PayPal",
                PayPalEmail = "test@example.com"
            };

            var result = PurchaseAndGetDto(controller, request);

            result.PaymentMethod.ShouldBe("PayPal");
            result.TransactionId.ShouldStartWith("PP-");
        }

        [Fact]
        public void Purchases_bundle_with_gift_card_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var request = new PurchaseCoinsBundleRequestDto
            {
                CoinsBundleId = 3,
                PaymentMethod = "GiftCard",
                GiftCardCode = "GIFT1234567890"
            };

            var result = PurchaseAndGetDto(controller, request);

            result.PaymentMethod.ShouldBe("GiftCard");
            result.TransactionId.ShouldStartWith("GC-");
        }

        [Fact]
        public void Purchase_fails_with_invalid_bundle_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var request = new PurchaseCoinsBundleRequestDto
            {
                CoinsBundleId = 9999,
                PaymentMethod = "CreditCard",
                CardNumber = "1234567890123456",
                CardHolderName = "Test User",
                ExpiryDate = "12/25",
                CVV = "123"
            };

            var result = controller.PurchaseBundle(request).Result;

            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Purchase_fails_with_invalid_credit_card_number()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var request = new PurchaseCoinsBundleRequestDto
            {
                CoinsBundleId = 1,
                PaymentMethod = "CreditCard",
                CardNumber = "123", 
                CardHolderName = "Test User",
                ExpiryDate = "12/25",
                CVV = "123"
            };

            var result = controller.PurchaseBundle(request).Result;

            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Purchase_fails_with_invalid_paypal_email()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var request = new PurchaseCoinsBundleRequestDto
            {
                CoinsBundleId = 1,
                PaymentMethod = "PayPal",
                PayPalEmail = "invalid-email" 
            };

            var result = controller.PurchaseBundle(request).Result;

            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Purchase_fails_with_invalid_gift_card_code()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var request = new PurchaseCoinsBundleRequestDto
            {
                CoinsBundleId = 1,
                PaymentMethod = "GiftCard",
                GiftCardCode = "SHORT" 
            };

            var result = controller.PurchaseBundle(request).Result;

            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Purchase_applies_discount_when_sale_is_active()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var request = new PurchaseCoinsBundleRequestDto
            {
                CoinsBundleId = 2,
                PaymentMethod = "CreditCard",
                CardNumber = "1234567890123456",
                CardHolderName = "Test User",
                ExpiryDate = "12/25",
                CVV = "123"
            };

            var result = PurchaseAndGetDto(controller, request);

            if (result.PricePaid < result.OriginalPrice)
            {
                result.PricePaid.ShouldBeLessThan(result.OriginalPrice);
            }
        }

        [Fact]
        public void Purchase_creates_wallet_if_not_exists()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-99"); 
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var request = new PurchaseCoinsBundleRequestDto
            {
                CoinsBundleId = 1,
                PaymentMethod = "CreditCard",
                CardNumber = "1234567890123456",
                CardHolderName = "New User",
                ExpiryDate = "12/25",
                CVV = "123"
            };

            var result = PurchaseAndGetDto(controller, request);

            var wallet = db.Wallets.FirstOrDefault(w => w.TouristId == -99);
            wallet.ShouldNotBeNull();
            wallet.Balance.ShouldBe(500m);
        }

        private static CoinsBundleController CreateTouristController(IServiceScope scope, string touristId)
        {
            return new CoinsBundleController(
                scope.ServiceProvider.GetRequiredService<ICoinsBundleService>())
            {
                ControllerContext = BuildContext(touristId)
            };
        }

        private static CoinsBundlePurchaseDto PurchaseAndGetDto(CoinsBundleController controller, PurchaseCoinsBundleRequestDto request)
        {
            var actionResult = controller.PurchaseBundle(request).Result;
            var okResult = actionResult as OkObjectResult;
            if (okResult != null)
            {
                var dto = okResult.Value as CoinsBundlePurchaseDto;
                if (dto != null) return dto;
            }
            var err = actionResult as ObjectResult;
            var msg = err?.Value?.ToString() ?? actionResult?.GetType().Name ?? "null";
            var status = err?.StatusCode ?? 0;
            throw new InvalidOperationException($"Purchase failed. StatusCode={status}, Response={msg}");
        }
    }
}