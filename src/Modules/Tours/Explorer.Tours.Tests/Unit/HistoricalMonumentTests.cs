using System;
using Explorer.Tours.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class HistoricalMonumentTests
    {
        private static HistoricalMonument CreateValid()
        {
            // TODO: ako ti ctor ima drugačiji potpis, pošalji mi pa prilagodim
            return new HistoricalMonument(
                name: "Monument",
                description: "Desc",
                yearOfCreation: 1900,
                latitude: 45.0,
                longitude: 19.0,
                administratorId: 1
            );
        }

        [Fact]
        public void Update_with_valid_data_updates_fields()
        {
            // Arrange
            var m = CreateValid();

            // Act
            m.Update(
                name: "Updated",
                description: "Updated desc",
                yearOfCreation: 2000,
                latitude: 44.5,
                longitude: 20.2,
                status: MonumentStatus.Active
            );

            // Assert
            m.Name.ShouldBe("Updated");
            m.Description.ShouldBe("Updated desc");
            m.YearOfCreation.ShouldBe(2000);
            m.Latitude.ShouldBe(44.5);
            m.Longitude.ShouldBe(20.2);
            m.Status.ShouldBe(MonumentStatus.Active);
        }

        [Fact]
        public void Update_fails_when_name_empty()
        {
            var m = CreateValid();

            var ex = Should.Throw<ArgumentException>(() =>
                m.Update("", "Desc", 1900, 45, 19, MonumentStatus.Active));

            ex.Message.ShouldContain("Name cannot be empty");
        }

        [Fact]
        public void Update_fails_when_description_empty()
        {
            var m = CreateValid();

            var ex = Should.Throw<ArgumentException>(() =>
                m.Update("Name", "", 1900, 45, 19, MonumentStatus.Active));

            ex.Message.ShouldContain("Description cannot be empty");
        }

        [Fact]
        public void Update_fails_when_year_not_positive()
        {
            var m = CreateValid();

            var ex = Should.Throw<ArgumentException>(() =>
                m.Update("Name", "Desc", 0, 45, 19, MonumentStatus.Active));

            ex.Message.ShouldContain("YearOfCreation must be greater than 0");
        }

        [Fact]
        public void Update_fails_when_latitude_out_of_range()
        {
            var m = CreateValid();

            var ex = Should.Throw<ArgumentException>(() =>
                m.Update("Name", "Desc", 1900, 100, 19, MonumentStatus.Active));

            ex.Message.ShouldContain("Latitude must be between -90 and 90");
        }

        [Fact]
        public void Update_fails_when_longitude_out_of_range()
        {
            var m = CreateValid();

            var ex = Should.Throw<ArgumentException>(() =>
                m.Update("Name", "Desc", 1900, 45, 200, MonumentStatus.Active));

            ex.Message.ShouldContain("Longitude must be between -180 and 180");
        }

        [Fact]
        public void Constructor_fails_when_administratorId_not_positive()
        {
            Should.Throw<ArgumentException>(() =>
                new HistoricalMonument(
                    name: "Monument",
                    description: "Desc",
                    yearOfCreation: 1900,
                    latitude: 45,
                    longitude: 19,
                    administratorId: 0
                )).Message.ShouldContain("AdministratorId must be greater than 0");
        }
    }
}
