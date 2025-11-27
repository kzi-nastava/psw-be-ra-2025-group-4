using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class HistoricalMonumentDTO
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int YearOfCreation { get; set; }
    public MonumentStatusDTO Status { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public long AdministratorId { get;  set; }
}

public enum MonumentStatusDTO
{
    Active,
    Inactive
}
