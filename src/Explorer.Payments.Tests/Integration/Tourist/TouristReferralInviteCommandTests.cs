using System;
using System.Linq;
using System.Collections.Generic;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TouristReferralInviteServiceTests : BasePaymentsIntegrationTest
    {
        public TouristReferralInviteServiceTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Invite_created_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITouristReferralInviteService>();
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            const long referrerTouristId = -21;

            var dto = service.Create(referrerTouristId);

            dto.ShouldNotBeNull();
            dto.Code.ShouldNotBeNullOrWhiteSpace();
            dto.IsUsed.ShouldBeFalse();

            db.ChangeTracker.Clear();
            db.TouristReferralInvites.Any(x => x.Code == dto.Code && x.ReferrerTouristId == referrerTouristId).ShouldBeTrue();
        }

        [Fact]
        public void Consume_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITouristReferralInviteService>();
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            const string code = "REFCONSUME32"; 
            const int referrerId = -21;        
            const int newTouristId = -23; 

            db.ChangeTracker.Clear();
            var beforeRef = db.Wallets.SingleOrDefault(w => w.TouristId == referrerId)?.Balance ?? 0m;
            var beforeNew = db.Wallets.SingleOrDefault(w => w.TouristId == newTouristId)?.Balance ?? 0m;

            service.ConsumeOnRegistration(code, newTouristId);

            db.ChangeTracker.Clear();

            db.Wallets.Single(w => w.TouristId == referrerId).Balance.ShouldBe(beforeRef + 3m);
            db.Wallets.Single(w => w.TouristId == newTouristId).Balance.ShouldBe(beforeNew + 3m);

            var invite = db.TouristReferralInvites.Single(x => x.Code == code);
            invite.IsUsed.ShouldBeTrue();
            invite.ReferredTouristId.ShouldBe(newTouristId);
        }

        [Fact]
        public void Consume_fails_when_code_does_not_exist()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITouristReferralInviteService>();

            Should.Throw<KeyNotFoundException>(() =>
                service.ConsumeOnRegistration("NO_SUCH_CODE", -22));
        }

        [Fact]
        public void Consume_returns_without_error_when_code_is_null_or_whitespace()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITouristReferralInviteService>();

            Should.NotThrow(() => service.ConsumeOnRegistration("   ", -22));
            Should.NotThrow(() => service.ConsumeOnRegistration(null, -22));
        }

        [Fact]
        public void Consume_fails_when_seed_code_is_already_used()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITouristReferralInviteService>();

            const string usedCode = "REFUSED31";

            Should.Throw<InvalidOperationException>(() =>
                service.ConsumeOnRegistration(usedCode, -23));
        }

        [Fact]
        public void Consume_fails_self_referral_using_seed_code()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITouristReferralInviteService>();

            Should.Throw<InvalidOperationException>(() =>
                service.ConsumeOnRegistration("REFCONSUME32", -21));
        }
    }
}
