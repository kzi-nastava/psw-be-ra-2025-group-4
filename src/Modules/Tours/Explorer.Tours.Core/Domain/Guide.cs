using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.Domain;

public class Guide : AggregateRoot
{
    public string Name { get; private set; }
    public List<string> Languages { get; private set; } = new();
    public decimal Price { get; private set; }
    public List<GuideTour> Tours { get; private set; } = new();

    private Guide() { }

    public Guide(string name, IEnumerable<string>? languages, decimal price)
    {
        Name = name?.Trim() ?? "";
        Languages = (languages ?? Enumerable.Empty<string>())
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        Price = price;

        Validate();
    }

    public void Update(string name, IEnumerable<string>? languages, decimal price)
    {
        Name = name?.Trim() ?? "";
        Languages = (languages ?? Enumerable.Empty<string>())
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        Price = price;

        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Guide name cannot be empty.");
        if (Price < 0) throw new ArgumentException("Price cannot be negative.");
    }
}
