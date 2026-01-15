using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Core.Domain.Repositories
{
    public interface IHiddenLocationEncounterRepository
    {
        HiddenLocationEncounter Create(HiddenLocationEncounter encounter);
        HiddenLocationEncounter Get(long id);
        HiddenLocationEncounter Update(HiddenLocationEncounter encounter);
    }
}
