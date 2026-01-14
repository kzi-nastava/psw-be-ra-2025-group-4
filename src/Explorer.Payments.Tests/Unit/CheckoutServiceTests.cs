using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.UseCases.Tourist;
using Explorer.Tours.API.Internal;
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

        private class WalletRepoStub : IWalletRepository
        {
            public Wallet? Wallet;

            public Wallet? GetByTouristId(int touristId)
                => Wallet != null && Wallet.TouristId == touristId ? Wallet : null;

            public Wallet Create(Wallet wallet) => Wallet = wallet;

            public Wallet Update(Wallet wallet)
            {
                Wallet = wallet;
                return wallet;
            }

            public bool Exists(int touristId)
                => Wallet != null && Wallet.TouristId == touristId;
        }

        private class PaymentRecordRepoStub : IPaymentRecordRepository
        {
            public readonly List<PaymentRecord> Store = new();

            public PaymentRecord Create(PaymentRecord paymentRecord)
            {
                Store.Add(paymentRecord);
                return paymentRecord;
            }

            public bool ExistsForBundle(int touristId, int bundleId)
            {
                return Store.Any(pr => pr.TouristId == touristId && pr.BundleId == bundleId);
            }
        }

        private class BundlePurchaseServiceStub : IBundlePurchaseService
        {
            public List<TourPurchaseTokenDto> PurchaseBundle(int touristId, int bundleId)
            {
                return new List<TourPurchaseTokenDto>();
            }
        }

        private class TourInfoServiceStub : ITourInfoService
        {
            private readonly Dictionary<int, decimal> _tourPrices = new();

            public void SetTourPrice(int tourId, decimal price)
            {
                _tourPrices[tourId] = price;
            }

            public TourInfoDto Get(int tourId)
            {
                var price = _tourPrices.ContainsKey(tourId) ? _tourPrices[tourId] : 0m;
                return new TourInfoDto
                {
                    TourId = tourId,
                    Name = $"Tour {tourId}",
                    Price = price,
                    Status = TourLifecycleStatus.Published
                };
            }
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
            var walletRepo = new WalletRepoStub { Wallet = new Wallet(123) };
            walletRepo.Wallet.AddBalance(100m);
            var paymentRecordRepo = new PaymentRecordRepoStub();
            var tourInfoService = new TourInfoServiceStub();
            tourInfoService.SetTourPrice(10, 20m);
            var svc = new CheckoutService(cartRepo, tokenRepo, walletRepo, paymentRecordRepo, tourInfoService, Mapper());
            var bundlePurchaseService = new BundlePurchaseServiceStub();
            var svc = new CheckoutService(cartRepo, tokenRepo, walletRepo, paymentRecordRepo, bundlePurchaseService, Mapper());

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
            var walletRepo = new WalletRepoStub { Wallet = new Wallet(123) };
            var paymentRecordRepo = new PaymentRecordRepoStub();
            var tourInfoService = new TourInfoServiceStub();
            var svc = new CheckoutService(cartRepo, tokenRepo, walletRepo, paymentRecordRepo, tourInfoService, Mapper());
            var bundlePurchaseService = new BundlePurchaseServiceStub();
            var svc = new CheckoutService(cartRepo, tokenRepo, walletRepo, paymentRecordRepo, bundlePurchaseService, Mapper());

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

            var walletRepo = new WalletRepoStub { Wallet = new Wallet(123) };
            walletRepo.Wallet.AddBalance(100m);
            var paymentRecordRepo = new PaymentRecordRepoStub();
            var tourInfoService = new TourInfoServiceStub();
            tourInfoService.SetTourPrice(10, 20m);
            tourInfoService.SetTourPrice(11, 30m);
            var svc = new CheckoutService(cartRepo, tokenRepo, walletRepo, paymentRecordRepo, tourInfoService, Mapper());
            var bundlePurchaseService = new BundlePurchaseServiceStub();
            var svc = new CheckoutService(cartRepo, tokenRepo, walletRepo, paymentRecordRepo, bundlePurchaseService, Mapper());
            var result = svc.Checkout(123);

            result.Count.ShouldBe(1);
            tokenRepo.Store.Count.ShouldBe(2);
        }

        [Fact]
        public void Checkout_throws_when_insufficient_balance()
        {
            var cartRepo = new CartRepoStub { Cart = new ShoppingCart(123) };
            cartRepo.Cart!.AddItem(10, "Tour A", 100m);

            var tokenRepo = new TokenRepoStub();
            var walletRepo = new WalletRepoStub { Wallet = new Wallet(123) };
            walletRepo.Wallet.AddBalance(50m);
            var paymentRecordRepo = new PaymentRecordRepoStub();
            var tourInfoService = new TourInfoServiceStub();
            tourInfoService.SetTourPrice(10, 100m);
            var svc = new CheckoutService(cartRepo, tokenRepo, walletRepo, paymentRecordRepo, tourInfoService, Mapper());
            var bundlePurchaseService = new BundlePurchaseServiceStub();
            var svc = new CheckoutService(cartRepo, tokenRepo, walletRepo, paymentRecordRepo, bundlePurchaseService, Mapper());

            Should.Throw<System.InvalidOperationException>(() => svc.Checkout(123));
        }

        [Fact]
        public void Checkout_creates_payment_record_for_each_item()
        {
            var cartRepo = new CartRepoStub { Cart = new ShoppingCart(123) };
            cartRepo.Cart!.AddItem(10, "Tour A", 20m);
            cartRepo.Cart.AddItem(11, "Tour B", 30m);

            var tokenRepo = new TokenRepoStub();
            var walletRepo = new WalletRepoStub { Wallet = new Wallet(123) };
            walletRepo.Wallet.AddBalance(100m);
            var paymentRecordRepo = new PaymentRecordRepoStub();
            var tourInfoService = new TourInfoServiceStub();
            tourInfoService.SetTourPrice(10, 20m);
            tourInfoService.SetTourPrice(11, 30m);
            var svc = new CheckoutService(cartRepo, tokenRepo, walletRepo, paymentRecordRepo, tourInfoService, Mapper());
            var bundlePurchaseService = new BundlePurchaseServiceStub();
            var svc = new CheckoutService(cartRepo, tokenRepo, walletRepo, paymentRecordRepo, bundlePurchaseService, Mapper());

            svc.Checkout(123);

            paymentRecordRepo.Store.Count.ShouldBe(2);
            paymentRecordRepo.Store[0].TourId.ShouldBe(10);
            paymentRecordRepo.Store[0].Price.ShouldBe(20m);
            paymentRecordRepo.Store[1].TourId.ShouldBe(11);
            paymentRecordRepo.Store[1].Price.ShouldBe(30m);
        }

        [Fact]
        public void Checkout_deducts_balance_from_wallet()
        {
            var cartRepo = new CartRepoStub { Cart = new ShoppingCart(123) };
            cartRepo.Cart!.AddItem(10, "Tour A", 20m);

            var tokenRepo = new TokenRepoStub();
            var walletRepo = new WalletRepoStub { Wallet = new Wallet(123) };
            walletRepo.Wallet.AddBalance(100m);
            var paymentRecordRepo = new PaymentRecordRepoStub();
            var tourInfoService = new TourInfoServiceStub();
            tourInfoService.SetTourPrice(10, 20m);
            var svc = new CheckoutService(cartRepo, tokenRepo, walletRepo, paymentRecordRepo, tourInfoService, Mapper());
            var bundlePurchaseService = new BundlePurchaseServiceStub();
            var svc = new CheckoutService(cartRepo, tokenRepo, walletRepo, paymentRecordRepo, bundlePurchaseService, Mapper());

            svc.Checkout(123);

            walletRepo.Wallet!.Balance.ShouldBe(80m);
        }
    }
}