using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        public UserProfileService(IUserProfileRepository profileRepository, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        public UserProfileDto Get(long userId)
        {
            var profile = _profileRepository.GetByUserId(userId);
            if (profile == null) throw new KeyNotFoundException("User profile not found.");

            return _mapper.Map<UserProfileDto>(profile);
        }

        public UserProfileDto Update(long userId, UpdateUserProfileDto dto)
        {
            var profile = _profileRepository.GetByUserId(userId);
            if (profile == null) throw new KeyNotFoundException("User profile not found.");

            
            profile.Update(
                dto.FirstName,
                dto.LastName,
                dto.Biography,
                dto.Motto,
                dto.ProfileImageUrl
            );

            var updated = _profileRepository.Update(profile);
            return _mapper.Map<UserProfileDto>(updated);
        }
    }
}
