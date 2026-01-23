using System.Security.Claims;
using Explorer.BuildingBlocks.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.Payments.Tests;

public class BasePaymentsIntegrationTest : BaseWebIntegrationTest<PaymentsTestFactory>
{
    protected BasePaymentsIntegrationTest(PaymentsTestFactory factory) : base(factory)
    {
    }

    protected static ControllerContext BuildContext(string userId)
    {
        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("personId", userId),
                    new Claim(ClaimTypes.Role, "author")
                }, "test"))
            }
        };
    }

    protected static ControllerContext BuildTouristContext(string userId)
    {
        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("personId", userId),
                    new Claim(ClaimTypes.Role, "tourist")
                }, "test"))
            }
        };
    }
}