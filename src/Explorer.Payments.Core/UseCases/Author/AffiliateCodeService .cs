using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Author;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Explorer.Payments.Core.UseCases.Author
{
    public class AffiliateCodeService : IAffiliateCodeService
    {
        private readonly IAffiliateCodeRepository _repo;
        private readonly ITourInfoService _tourInfoService;
        private readonly IMapper _mapper;

        public AffiliateCodeService(
            IAffiliateCodeRepository repo,
            ITourInfoService tourInfoService,
            IMapper mapper)
        {
            _repo = repo;
            _tourInfoService = tourInfoService;
            _mapper = mapper;
        }

        public AffiliateCodeDto Create(CreateAffiliateCodeDto dto, int authorId)
        {
            if (dto == null) throw new ArgumentException("Body is required.");

            var tourId = dto.TourId;

            if (tourId.HasValue)
            {
                var tour = _tourInfoService.Get(tourId.Value);
                if (tour.AuthorId != authorId)
                    throw new ForbiddenException("Not your tour.");
            }

            if (dto.AffiliateTouristId == 0) throw new ArgumentException("AffiliateTouristId is required.");
            if (dto.Percent <= 0 || dto.Percent > 100) throw new ArgumentException("Percent must be in (0, 100].");
            if (dto.ExpiresAt.HasValue && dto.ExpiresAt.Value <= DateTime.UtcNow)
                throw new ArgumentException("ExpiresAt must be in the future.");

            for (var attempt = 0; attempt < 10; attempt++)
            {
                var code = GenerateCode(10);
                if (_repo.CodeExists(code)) continue;

                try
                {
                    var created = _repo.Create(new AffiliateCode(
                        code,
                        authorId,
                        tourId,
                        dto.AffiliateTouristId,
                        dto.Percent,
                        dto.ExpiresAt
                    ));

                    return _mapper.Map<AffiliateCodeDto>(created);
                }
                catch
                {
                    // unique race -> retry
                }
            }

            throw new Exception("Unable to generate unique affiliate code.");
        }

        public List<AffiliateCodeDto> GetAll(int authorId, int? tourId = null)
        {
            var items = _repo.GetByAuthor(authorId, tourId)
                             .OrderByDescending(x => x.CreatedAt)
                             .ToList();

            return items.Select(_mapper.Map<AffiliateCodeDto>).ToList();
        }

        public void Deactivate(int authorId, int affiliateCodeId)
        {
            var code = _repo.GetById(affiliateCodeId);
            if (code == null) throw new NotFoundException("Affiliate code not found.");

            if (code.AuthorId != authorId)
                throw new ForbiddenException("Not your affiliate code.");

            if (!code.Active) return;

            code.Deactivate();
            _repo.SaveChanges();
        }

        private static string GenerateCode(int length)
        {
            const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var bytes = RandomNumberGenerator.GetBytes(length);
            var chars = bytes.Select(b => alphabet[b % alphabet.Length]).ToArray();
            return new string(chars);
        }
    }
}
