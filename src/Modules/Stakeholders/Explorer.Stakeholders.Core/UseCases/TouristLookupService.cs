using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public class TouristLookupService : ITouristLookupService
{
    private readonly IUserRepository _userRepository;

    public TouristLookupService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public List<TouristLookupDto> GetAll(bool onlyActive = true)
    {
        return _userRepository
            .GetTourists(onlyActive)
            .OrderBy(u => u.Username)
            .Select(u => new TouristLookupDto
            {
                Id = u.Id,
                Username = u.Username,
                IsActive = u.IsActive
            })
            .ToList();
    }
}
