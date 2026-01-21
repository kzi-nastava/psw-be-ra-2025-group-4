namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface IEncounterExecutionRepository
    {
        EncounterExecution? Get(long touristId, long encounterId);
        bool Exists(long touristId, long encounterId);

        EncounterExecution Create(EncounterExecution execution);
        EncounterExecution Update(EncounterExecution execution);
        IEnumerable<EncounterExecution> GetActiveExecutions(long encounterId);
    }
}
