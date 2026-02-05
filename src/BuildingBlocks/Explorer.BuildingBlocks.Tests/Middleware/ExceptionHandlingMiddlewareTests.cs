using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Explorer.API.Middleware;
using Explorer.BuildingBlocks.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Xunit;

namespace Explorer.BuildingBlocks.Tests.Middleware
{
    public class ExceptionHandlingMiddlewareTests
    {
        private async Task<(int StatusCode, string ContentType, JsonDocument Json)> InvokeMiddlewareAsync(Exception exception)
        {
            RequestDelegate next = _ => Task.FromException(exception);

            var context = new DefaultHttpContext();
            var ms = new MemoryStream();
            context.Response.Body = ms;

            var logger = NullLogger<ExceptionHandlingMiddleware>.Instance;
            var middleware = new ExceptionHandlingMiddleware(next, logger);

            await middleware.InvokeAsync(context);

            ms.Seek(0, SeekOrigin.Begin);
            var json = await JsonDocument.ParseAsync(ms);

            return (context.Response.StatusCode, context.Response.ContentType ?? string.Empty, json);
        }

        [Fact]
        public async Task ArgumentException_returns_400_and_message()
        {
            var (status, contentType, json) = await InvokeMiddlewareAsync(new ArgumentException("invalid param"));

            status.ShouldBe(400);
            contentType.ShouldBe("application/json");
            json.RootElement.GetProperty("title").GetString().ShouldBe("An error occurred");
            json.RootElement.GetProperty("status").GetInt32().ShouldBe(400);
            json.RootElement.GetProperty("detail").GetString().ShouldBe("invalid param");
        }

        [Fact]
        public async Task UnauthorizedAccessException_returns_401_and_message()
        {
            var (status, contentType, json) = await InvokeMiddlewareAsync(new UnauthorizedAccessException("no access"));

            status.ShouldBe(401);
            contentType.ShouldBe("application/json");
            json.RootElement.GetProperty("status").GetInt32().ShouldBe(401);
            json.RootElement.GetProperty("detail").GetString().ShouldBe("no access");
        }

        [Fact]
        public async Task ForbiddenException_returns_403_and_message()
        {
            var (status, contentType, json) = await InvokeMiddlewareAsync(new ForbiddenException("forbidden"));

            status.ShouldBe(403);
            contentType.ShouldBe("application/json");
            json.RootElement.GetProperty("status").GetInt32().ShouldBe(403);
            json.RootElement.GetProperty("detail").GetString().ShouldBe("forbidden");
        }

        [Fact]
        public async Task NotFoundException_returns_404_and_message()
        {
            var (status, contentType, json) = await InvokeMiddlewareAsync(new NotFoundException("not found"));

            status.ShouldBe(404);
            contentType.ShouldBe("application/json");
            json.RootElement.GetProperty("status").GetInt32().ShouldBe(404);
            json.RootElement.GetProperty("detail").GetString().ShouldBe("not found");
        }

        [Fact]
        public async Task EntityValidationException_returns_422_and_message()
        {
            var (status, contentType, json) = await InvokeMiddlewareAsync(new EntityValidationException("validation failed"));

            status.ShouldBe(422);
            contentType.ShouldBe("application/json");
            json.RootElement.GetProperty("status").GetInt32().ShouldBe(422);
            json.RootElement.GetProperty("detail").GetString().ShouldBe("validation failed");
        }

        [Fact]
        public async Task GenericException_returns_500_and_generic_message()
        {
            var (status, contentType, json) = await InvokeMiddlewareAsync(new Exception("boom"));

            status.ShouldBe(500);
            contentType.ShouldBe("application/json");
            json.RootElement.GetProperty("status").GetInt32().ShouldBe(500);
            json.RootElement.GetProperty("detail").GetString().ShouldBe("An internal server error occurred.");
        }
    }
}
