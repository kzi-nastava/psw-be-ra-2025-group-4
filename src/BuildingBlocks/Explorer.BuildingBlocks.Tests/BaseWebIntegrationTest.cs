using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Security.Claims;
using Explorer.API;
using Xunit;
using System.Net.Http;

namespace Explorer.BuildingBlocks.Tests;

public class BaseWebIntegrationTest<TTestFactory> : IClassFixture<TTestFactory>
    where TTestFactory : WebApplicationFactory<Program>
{
    protected readonly TTestFactory Factory;
    protected readonly HttpClient Client;

    public BaseWebIntegrationTest(TTestFactory factory)
    {
        Factory = factory;

        
        Client = factory.CreateClient();
    }

    protected static ControllerContext BuildContext(string id)
    {
        return new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("personId", id)
                }))
            }
        };
    }
}
