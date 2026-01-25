using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Dtos.Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Administration;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases.Tourist
{
    public class TouristReferralInviteService : ITouristReferralInviteService
    {
        private const decimal RewardAmount = 10m;

        private readonly ITouristReferralInviteRepository _repo;
        private readonly IWalletAdministrationService _walletAdmin;

        public TouristReferralInviteService(
            ITouristReferralInviteRepository repo,
            IWalletAdministrationService walletAdmin)
        {
            _repo = repo;
            _walletAdmin = walletAdmin;
        }

        public TouristReferralInviteDto Create(long referrerTouristId)
        {
            for (var attempt = 0; attempt < 10; attempt++)
            {
                var code = GenerateCode(10);
                if (_repo.CodeExists(code)) continue;

                var created = _repo.Create(new TouristReferralInvite(code, referrerTouristId));
                return new TouristReferralInviteDto
                {
                    Code = created.Code,
                    IsUsed = created.IsUsed
                };
            }

            throw new Exception("Unable to generate unique referral code.");
        }

        public void ConsumeOnRegistration(string code, long newTouristId)
        {
            if (string.IsNullOrWhiteSpace(code)) return;

            var invite = _repo.GetByCode(code);
            if (invite == null) throw new KeyNotFoundException("Referral code not found.");

            invite.Consume(newTouristId);

            _repo.Update(invite);

            _walletAdmin.AddBalance((int)invite.ReferrerTouristId, RewardAmount);
            _walletAdmin.AddBalance((int)newTouristId, RewardAmount);
        }

        private static string GenerateCode(int length)
        {
            const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var rng = Random.Shared;

            return new string(Enumerable.Range(0, length)
                .Select(_ => alphabet[rng.Next(alphabet.Length)])
                .ToArray());
        }
    }
}
