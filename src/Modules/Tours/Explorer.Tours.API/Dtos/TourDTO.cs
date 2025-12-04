using System;
using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos
{
    public enum TourDtoDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public enum TourDtoStatus
    {
        Draft,
        Published,
        Archived
    }

    public class TourDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TourDtoDifficulty Difficulty { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public TourDtoStatus Status { get; set; } = TourDtoStatus.Draft;   
        public decimal Price { get; set; }
        public int AuthorId { get; set; }
        public List<TourPointDto> Points { get; set; }
        public List<TourTransportDurationDto> TransportDuration { get; set; }

    }
}
