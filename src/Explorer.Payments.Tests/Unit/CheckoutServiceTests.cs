using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.UseCases.Tourist;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Unit
{
    public class CheckoutServiceTests
    {
        private class CartRepoStub : IShoppingCartRepository
        {
            public ShoppingCart? Cart;

            public ShoppingCart? GetByTouristId(int touristId)
                => Cart != null && Cart.TouristId == touristId ? Cart : null;

            public ShoppingCart Create(ShoppingCart cart) => Cart = cart;

            public ShoppingCart Update(ShoppingCart cart)
            {
                Cart = cart;
                return cart;
            }
        }

        private class TokenRepoStub : ITourPurchaseTokenRepository
        {
            public readonly List<TourPurchaseToken> Store = new();

            public TourPurchaseToken Create(TourPurchaseToken token)
            {
                Store.Add(token);
                return token;
            }

            public List<TourPurchaseToken> GetByTouristId(int touristId)
                => Store.Where(t => t.TouristId == touristId).ToList();

            public bool Exists(int touristId, int tourId)
                => Store.Any(t => t.TouristId == touristId && t.TourId == tourId);
        }

        private static IMapper Mapper()
        {
            var cfg = new MapperConfiguration(c =>
            {
                c.CreateMap<TourPurchaseToken, TourPurchaseTokenDto>().ReverseMap();
            });
            return cfg.CreateMapper();
        }

        [Fact]
        public void Checkout_creates_tokens_and_clears_cart()
        {
            var cartRepo = new CartRepoStub { Cart = new ShoppingCart(123) };
            cartRepo.Cart!.AddItem(10, "Tour A", 20m);

            var tokenRepo = new TokenRepoStub();
            var svc = new CheckoutService(cartRepo, tokenRepo, Mapper());

            var result = svc.Checkout(123);

            result.Count.ShouldBe(1);
            tokenRepo.Store.Count.ShouldBe(1);
            cartRepo.Cart!.Items.Count.ShouldBe(0);
            cartRepo.Cart.TotalPrice.ShouldBe(0m);
        }

        [Fact]
        public void Checkout_throws_when_cart_is_empty()
        {
            var cartRepo = new CartRepoStub { Cart = new ShoppingCart(123) };
            var tokenRepo = new TokenRepoStub();
            var svc = new CheckoutService(cartRepo, tokenRepo, Mapper());

            Should.Throw<System.InvalidOperationException>(() => svc.Checkout(123));
        }

        [Fact]
        public void Checkout_skips_already_existing_token()
        {
            var cartRepo = new CartRepoStub { Cart = new ShoppingCart(123) };
            cartRepo.Cart!.AddItem(10, "Tour A", 20m);
            cartRepo.Cart.AddItem(11, "Tour B", 30m);

            var tokenRepo = new TokenRepoStub();
            tokenRepo.Create(new TourPurchaseToken(123, 10));

            var svc = new CheckoutService(cartRepo, tokenRepo, Mapper());
            var result = svc.Checkout(123);

            result.Count.ShouldBe(1);
            tokenRepo.Store.Count.ShouldBe(2);
        }
    }
}