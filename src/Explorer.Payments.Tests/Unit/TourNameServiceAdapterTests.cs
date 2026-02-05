using System;
using Explorer.Payments.Infrastructure.Internal;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Unit
{
    public class TourNameServiceAdapterTests
    {
        

        [Fact]
        public void Constructor_throws_when_service_is_null()
        {
            Should.Throw<ArgumentNullException>(() =>
                new TourNameServiceAdapter(null!)
            );
        }

        

        [Fact]
        public void GetTourName_returns_name_when_service_returns_dto_with_name()
        {
            var fakeService = new FakeTourInfoService();
            var adapter = new TourNameServiceAdapter(fakeService);

            var result = adapter.GetTourName(1);

            result.ShouldBe("Test Tour");
        }

       

        [Fact]
        public void GetTourName_returns_null_when_Get_method_does_not_exist()
        {
            var adapter = new TourNameServiceAdapter(new ObjectWithoutGet());

            var result = adapter.GetTourName(1);

            result.ShouldBeNull();
        }

        [Fact]
        public void GetTourName_returns_null_when_Get_returns_null()
        {
            var adapter = new TourNameServiceAdapter(new GetReturnsNullService());

            var result = adapter.GetTourName(1);

            result.ShouldBeNull();
        }

        [Fact]
        public void GetTourName_returns_null_when_dto_has_no_name_property()
        {
            var adapter = new TourNameServiceAdapter(new GetReturnsDtoWithoutName());

            var result = adapter.GetTourName(1);

            result.ShouldBeNull();
        }

        [Fact]
        public void GetTourName_returns_null_when_exception_is_thrown()
        {
            var adapter = new TourNameServiceAdapter(new ThrowingService());

            var result = adapter.GetTourName(1);

            result.ShouldBeNull();
        }

       

        private class FakeTourInfoService
        {
            public object Get(int id)
            {
                return new { Name = "Test Tour" };
            }
        }

        private class ObjectWithoutGet
        {
        }

        private class GetReturnsNullService
        {
            public object? Get(int id) => null;
        }

        private class GetReturnsDtoWithoutName
        {
            public object Get(int id)
            {
                return new { Id = id };
            }
        }

        private class ThrowingService
        {
            public object Get(int id)
            {
                throw new Exception("boom");
            }
        }
    }
}
