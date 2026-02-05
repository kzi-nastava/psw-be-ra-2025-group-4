using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos;

public class GuideDto
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public List<string> Languages { get; set; } = new();
    public decimal Price { get; set; }
}
