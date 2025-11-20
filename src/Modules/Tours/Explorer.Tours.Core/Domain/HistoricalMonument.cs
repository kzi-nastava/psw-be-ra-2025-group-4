using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public enum MonumentStatus
{
    Active,
    Inactive
}
public class HistoricalMonument : Entity
{
    public long Id { get; set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int YearOfCreation { get; private set; }
    public MonumentStatus Status { get; private set; } = MonumentStatus.Active;
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public long AdministratorId { get; private set; }

    public HistoricalMonument(string name, string description, int yearOfCreation, double latitude, double longitude, long administratorId)
    {
        Name = name;
        Description = description;
        YearOfCreation = yearOfCreation;
        Latitude = latitude;
        Longitude = longitude;
        AdministratorId = administratorId;
        Status = MonumentStatus.Active;

        Validate();
    }

    public void Update(string name, string description, int yearOfCreation, double latitude, double longitude, MonumentStatus status)
    {
        Name = name;
        Description = description;
        YearOfCreation = yearOfCreation;
        Latitude = latitude;
        Longitude = longitude;
        Status = status;

        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Name cannot be empty");
        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Description cannot be empty");
        if (YearOfCreation <= 0)
            throw new ArgumentException("YearOfCreation must be greater than 0");
        if (Latitude < -90 || Latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90");
        if (Longitude < -180 || Longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180");
        if (AdministratorId <= 0)
            throw new ArgumentException("AdministratorId must be greater than 0");
    }
}
