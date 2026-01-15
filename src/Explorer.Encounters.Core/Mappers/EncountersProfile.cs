using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Core.Mappers;

public class EncountersProfile : Profile
{
    public EncountersProfile()
    {
        CreateMap<Encounter, EncounterDto>().ReverseMap();
        CreateMap<Location, LocationDto>().ReverseMap();
        CreateMap<HiddenLocationEncounter, HiddenLocationEncounterDto>().ReverseMap();
        CreateMap<EncounterExecution, EncounterExecutionDto>().ReverseMap();
    }
}
