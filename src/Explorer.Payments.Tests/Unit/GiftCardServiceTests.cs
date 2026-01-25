using System.Collections.Generic;
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Mappers;
using Explorer.Payments.Core.UseCases.Tourist;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Unit
{
    public class GiftCardServiceTests
    {
        private class GiftCardRepoStub : IGiftCardRepository
        {
            public readonly List<GiftCard> Store = new();

            public GiftCard? GetByCode(string code) => Store.Find(g => g.Code == code);
            public List<GiftCard> GetByRecipientTouristId(int touristId) =>
                Store.FindAll(g => g.RecipientTouristId == touristId && g.Balance > 0);
            public GiftCard Create(GiftCard g) { Store.Add(g); return g; }
            public GiftCard Update(GiftCard g) => g;
            public bool ExistsCode(string code) => Store.Exists(g => g.Code == code);
        }

        private class UserInfoServiceStub : IUserInfoService
        {
            private long? _personIdForUsername;

            public void SetPersonIdForUsername(string username, long? personId) => _personIdForUsername = personId;

            public UserInfo? GetUser(long userId) => null;
            public UserInfo? GetUserByUsername(string username) => null;
            public long? GetPersonIdByUsername(string username) => _personIdForUsername;
            public bool IsAdministrator(long userId) => false;
        }

        private static IMapper Mapper()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile<PaymentsProfile>());
            return cfg.CreateMapper();
        }

        [Fact]
        public void PurchaseGiftCard_throws_when_recipient_username_empty()
        {
            var repo = new GiftCardRepoStub();
            var userInfo = new UserInfoServiceStub();
            var svc = new GiftCardService(repo, userInfo, Mapper());

            Should.Throw<ArgumentException>(() => svc.PurchaseGiftCard(1, new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "",
                Amount = 10m,
                PaymentMethod = "CreditCard",
                CardNumber = "1234567890123456",
                CardHolderName = "Test",
                ExpiryDate = "12/28",
                CVV = "123"
            })).Message.ShouldContain("Recipient username");
        }

        [Fact]
        public void PurchaseGiftCard_throws_when_amount_not_positive()
        {
            var repo = new GiftCardRepoStub();
            var userInfo = new UserInfoServiceStub();
            userInfo.SetPersonIdForUsername("joe", 2);
            var svc = new GiftCardService(repo, userInfo, Mapper());

            Should.Throw<ArgumentException>(() => svc.PurchaseGiftCard(1, new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "joe",
                Amount = 0m,
                PaymentMethod = "CreditCard",
                CardNumber = "1234567890123456",
                CardHolderName = "Test",
                ExpiryDate = "12/28",
                CVV = "123"
            })).Message.ShouldContain("Amount");
        }

        [Fact]
        public void PurchaseGiftCard_throws_when_recipient_not_found()
        {
            var repo = new GiftCardRepoStub();
            var userInfo = new UserInfoServiceStub();
            userInfo.SetPersonIdForUsername("nobody", null);
            var svc = new GiftCardService(repo, userInfo, Mapper());

            Should.Throw<ArgumentException>(() => svc.PurchaseGiftCard(1, new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "nobody",
                Amount = 10m,
                PaymentMethod = "CreditCard",
                CardNumber = "1234567890123456",
                CardHolderName = "Test",
                ExpiryDate = "12/28",
                CVV = "123"
            })).Message.ShouldContain("not found");
        }

        [Fact]
        public void PurchaseGiftCard_throws_when_buy_for_self()
        {
            var repo = new GiftCardRepoStub();
            var userInfo = new UserInfoServiceStub();
            userInfo.SetPersonIdForUsername("me", 1);
            var svc = new GiftCardService(repo, userInfo, Mapper());

            Should.Throw<ArgumentException>(() => svc.PurchaseGiftCard(1, new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "me",
                Amount = 10m,
                PaymentMethod = "CreditCard",
                CardNumber = "1234567890123456",
                CardHolderName = "Test",
                ExpiryDate = "12/28",
                CVV = "123"
            })).Message.ShouldContain("yourself");
        }

        [Fact]
        public void PurchaseGiftCard_throws_when_invalid_payment_method()
        {
            var repo = new GiftCardRepoStub();
            var userInfo = new UserInfoServiceStub();
            userInfo.SetPersonIdForUsername("joe", 2);
            var svc = new GiftCardService(repo, userInfo, Mapper());

            Should.Throw<ArgumentException>(() => svc.PurchaseGiftCard(1, new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "joe",
                Amount = 10m,
                PaymentMethod = "GiftCard"
            })).Message.ShouldContain("Invalid payment method");
        }

        [Fact]
        public void PurchaseGiftCard_throws_when_credit_card_invalid()
        {
            var repo = new GiftCardRepoStub();
            var userInfo = new UserInfoServiceStub();
            userInfo.SetPersonIdForUsername("joe", 2);
            var svc = new GiftCardService(repo, userInfo, Mapper());

            Should.Throw<ArgumentException>(() => svc.PurchaseGiftCard(1, new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "joe",
                Amount = 10m,
                PaymentMethod = "CreditCard",
                CardNumber = "123"
            })).Message.ShouldContain("card");
        }

        [Fact]
        public void PurchaseGiftCard_throws_when_paypal_invalid()
        {
            var repo = new GiftCardRepoStub();
            var userInfo = new UserInfoServiceStub();
            userInfo.SetPersonIdForUsername("joe", 2);
            var svc = new GiftCardService(repo, userInfo, Mapper());

            Should.Throw<ArgumentException>(() => svc.PurchaseGiftCard(1, new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "joe",
                Amount = 10m,
                PaymentMethod = "PayPal",
                PayPalEmail = "not-an-email"
            })).Message.ShouldContain("PayPal");
        }

        [Fact]
        public void PurchaseGiftCard_succeeds_with_valid_credit_card()
        {
            var repo = new GiftCardRepoStub();
            var userInfo = new UserInfoServiceStub();
            userInfo.SetPersonIdForUsername("joe", 2);
            var svc = new GiftCardService(repo, userInfo, Mapper());

            var result = svc.PurchaseGiftCard(1, new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "joe",
                Amount = 25m,
                PaymentMethod = "CreditCard",
                CardNumber = "1234567890123456",
                CardHolderName = "Buyer",
                ExpiryDate = "12/28",
                CVV = "456"
            });

            result.Amount.ShouldBe(25m);
            result.Balance.ShouldBe(25m);
            result.RecipientTouristId.ShouldBe(2);
            result.Code.Length.ShouldBeGreaterThanOrEqualTo(10);
            repo.Store.Count.ShouldBe(1);
            repo.Store[0].RecipientTouristId.ShouldBe(2);
            repo.Store[0].BuyerTouristId.ShouldBe(1);
        }

        [Fact]
        public void PurchaseGiftCard_succeeds_with_valid_paypal()
        {
            var repo = new GiftCardRepoStub();
            var userInfo = new UserInfoServiceStub();
            userInfo.SetPersonIdForUsername("jane", 3);
            var svc = new GiftCardService(repo, userInfo, Mapper());

            var result = svc.PurchaseGiftCard(1, new PurchaseGiftCardRequestDto
            {
                RecipientUsername = "jane",
                Amount = 15m,
                PaymentMethod = "PayPal",
                PayPalEmail = "jane@example.com"
            });

            result.Amount.ShouldBe(15m);
            result.Balance.ShouldBe(15m);
            result.RecipientTouristId.ShouldBe(3);
            repo.Store.Count.ShouldBe(1);
        }

        [Fact]
        public void GetMyGiftCards_returns_empty_when_none()
        {
            var repo = new GiftCardRepoStub();
            var userInfo = new UserInfoServiceStub();
            var svc = new GiftCardService(repo, userInfo, Mapper());

            var result = svc.GetMyGiftCards(99);

            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }

        [Fact]
        public void GetMyGiftCards_returns_only_cards_with_balance_for_recipient()
        {
            var repo = new GiftCardRepoStub();
            repo.Create(new GiftCard("CARD1234567", 10, 20m, 1));
            var userInfo = new UserInfoServiceStub();
            var svc = new GiftCardService(repo, userInfo, Mapper());

            var result = svc.GetMyGiftCards(10);

            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].Code.ShouldBe("CARD1234567");
            result[0].Balance.ShouldBe(20m);
        }
    }
}
