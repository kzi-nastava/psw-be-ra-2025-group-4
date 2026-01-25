using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases.Tourist
{
    public class GiftCardService : IGiftCardService
    {
        public const int GiftCardCodeLength = 12;
        private const string CodeChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private readonly IGiftCardRepository _giftCardRepository;
        private readonly IUserInfoService _userInfoService;
        private readonly IMapper _mapper;
        private readonly Random _random = new();

        public GiftCardService(
            IGiftCardRepository giftCardRepository,
            IUserInfoService userInfoService,
            IMapper mapper)
        {
            _giftCardRepository = giftCardRepository;
            _userInfoService = userInfoService;
            _mapper = mapper;
        }

        public GiftCardDto PurchaseGiftCard(int buyerTouristId, PurchaseGiftCardRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.RecipientUsername))
                throw new ArgumentException("Recipient username is required.");
            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be positive.");

            var recipientPersonId = _userInfoService.GetPersonIdByUsername(request.RecipientUsername.Trim());
            if (recipientPersonId == null)
                throw new ArgumentException("Recipient username not found.");
            var recipientId = (int)recipientPersonId.Value;
            if (recipientId == buyerTouristId)
                throw new ArgumentException("You cannot buy a gift card for yourself.");

            if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, out var paymentMethod) ||
                (paymentMethod != PaymentMethod.CreditCard && paymentMethod != PaymentMethod.PayPal))
                throw new ArgumentException("Invalid payment method. Use CreditCard or PayPal.");

            ValidatePaymentDetails(request, paymentMethod);

            var code = GenerateUniqueCode();
            var giftCard = new GiftCard(code, recipientId, request.Amount, buyerTouristId);
            _giftCardRepository.Create(giftCard);

            var dto = _mapper.Map<GiftCardDto>(giftCard);
            dto.RecipientTouristId = recipientId;
            return dto;
        }

        public List<GiftCardDto> GetMyGiftCards(int touristId)
        {
            var cards = _giftCardRepository.GetByRecipientTouristId(touristId);
            return _mapper.Map<List<GiftCardDto>>(cards);
        }

        private void ValidatePaymentDetails(PurchaseGiftCardRequestDto request, PaymentMethod method)
        {
            switch (method)
            {
                case PaymentMethod.CreditCard:
                    if (string.IsNullOrWhiteSpace(request.CardNumber) || request.CardNumber.Length < 13)
                        throw new ArgumentException("Invalid card number.");
                    if (string.IsNullOrWhiteSpace(request.CardHolderName))
                        throw new ArgumentException("Card holder name is required.");
                    if (string.IsNullOrWhiteSpace(request.ExpiryDate))
                        throw new ArgumentException("Expiry date is required.");
                    if (string.IsNullOrWhiteSpace(request.CVV) || request.CVV.Length < 3)
                        throw new ArgumentException("Invalid CVV.");
                    break;
                case PaymentMethod.PayPal:
                    if (string.IsNullOrWhiteSpace(request.PayPalEmail) || !request.PayPalEmail.Contains("@"))
                        throw new ArgumentException("Valid PayPal email is required.");
                    break;
                default:
                    throw new ArgumentException("Invalid payment method.");
            }
        }

        private string GenerateUniqueCode()
        {
            for (int attempt = 0; attempt < 100; attempt++)
            {
                var code = new string(Enumerable.Range(0, GiftCardCodeLength)
                    .Select(_ => CodeChars[_random.Next(CodeChars.Length)]).ToArray());
                if (!_giftCardRepository.ExistsCode(code))
                    return code;
            }
            return "GC" + Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();
        }
    }
}
