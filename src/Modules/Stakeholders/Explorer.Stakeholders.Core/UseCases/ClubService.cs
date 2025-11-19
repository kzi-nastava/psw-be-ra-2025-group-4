using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IMapper _mapper;

        public ClubService(IClubRepository clubRepository, IMapper mapper)
        {
            _clubRepository = clubRepository;
            _mapper = mapper;
        }

        public ClubDto Create(ClubDto clubDto)
        {
            var club = _mapper.Map<Club>(clubDto);
            var created = _clubRepository.Create(club);
            return _mapper.Map<ClubDto>(created);
        }

        public ClubDto Update(long clubId, ClubDto clubDto)
        {
            var club = _clubRepository.GetById(clubId);
            if (club == null) throw new KeyNotFoundException("Club not found");

            if (club.OwnerId != clubDto.OwnerId)
                throw new UnauthorizedAccessException("Only owner can update the club.");

            club.Update(clubDto.Name, clubDto.Description, clubDto.ImageUrls);
            var updated = _clubRepository.Update(club);

            return _mapper.Map<ClubDto>(updated);
        }

        public void Delete(long clubId, long ownerId)
        {
            var club = _clubRepository.GetById(clubId);
            if (club == null) throw new KeyNotFoundException("Club not found");

            if (club.OwnerId != ownerId)
                throw new UnauthorizedAccessException("Only owner can delete the club.");

            _clubRepository.Delete(clubId);
        }

        public List<ClubDto> GetAll()
        {
            var clubs = _clubRepository.GetAll();
            return _mapper.Map<List<ClubDto>>(clubs);
        }

        public List<ClubDto> GetByOwner(long ownerId)
        {
            var clubs = _clubRepository.GetByOwner(ownerId);
            return _mapper.Map<List<ClubDto>>(clubs);
        }
    }
}
