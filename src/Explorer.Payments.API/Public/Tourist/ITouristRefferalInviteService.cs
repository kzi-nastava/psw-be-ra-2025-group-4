using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Dtos.Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public.Tourist
{
    public interface ITouristReferralInviteService
    {
        TouristReferralInviteDto Create(long referrerTouristId);
        void ConsumeOnRegistration(string code, long newTouristId);
    }
}
