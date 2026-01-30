using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ITouristReferralInviteRepository
    {
        TouristReferralInvite? GetByCode(string code);
        bool CodeExists(string code);
        TouristReferralInvite Create(TouristReferralInvite invite);
        TouristReferralInvite Update(TouristReferralInvite invite);
    }
}
