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
        private readonly INotificationService _notificationService;

        public ClubService(IClubRepository clubRepository, IMapper mapper, INotificationService notificationService)
        {
            _clubRepository = clubRepository;
            _mapper = mapper;
            _notificationService = notificationService;
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
        public List<ClubDto> GetInvitesForMe(long touristId)
        {
            return _clubRepository.GetAll()
                .Where(c => c.InvitedTourist != null && c.InvitedTourist.Contains(touristId))
                .Select(c => _mapper.Map<ClubDto>(c))
                .ToList();
        }
        public void InviteMember(long clubId, long ownerId, long touristId)
        {
            var club = _clubRepository.GetById(clubId);
            club.InviteMember(ownerId, touristId);

            _clubRepository.Update(club);
        }

        public void AcceptInvite(long clubId, long touristId)
        {
            var club = _clubRepository.GetById(clubId);
            club.AcceptInvite(touristId);
            _clubRepository.Update(club);
        }

        public void RemoveMember(long clubId, long ownerId, long touristId)
        {
            var club = _clubRepository.GetById(clubId);
            club.RemoveMember(ownerId, touristId);
            _clubRepository.Update(club);
        }

        public void CloseClub(long clubId, long ownerId)
        {
            var club = _clubRepository.GetById(clubId);
            club.Close(ownerId);
            _clubRepository.Update(club);
        }

        public void ActivateClub(long clubId, long ownerId)
        {
            var club = _clubRepository.GetById(clubId);
            club.Activate(ownerId);
            _clubRepository.Update(club);
        }
        public void RequestToJoinClub(long clubId, long touristId)
        {
            var club = _clubRepository.GetById(clubId);
            club.RequestToJoin(touristId);
            _clubRepository.Update(club);
        }
        public void CancelJoinRequest(long clubId, long touristId)
        {
            var club = _clubRepository.GetById(clubId);
            club.CancelJoinRequest(touristId);
            _clubRepository.Update(club);
        }
        public void AcceptJoinRequest(long clubId, long ownerId, long touristId)
        {
            var club = _clubRepository.GetById(clubId);
            club.AcceptJoinRequest(ownerId, touristId);
            _clubRepository.Update(club);
            // ✅ Notifikacija: "Prihvaćen zahtev"
            _notificationService.CreateClubJoinRequestResponseNotification(
                userId: touristId,
                actorId: ownerId,
                clubId: clubId,
                clubName: club.Name,
                accepted: true
            );
        }
        public void DeclineJoinRequest(long clubId, long ownerId, long touristId)
        {
            var club = _clubRepository.GetById(clubId);
            club.DeclineJoinRequest(ownerId, touristId);
            _clubRepository.Update(club);
            // ✅ Notifikacija: "Odbijen zahtev"
            _notificationService.CreateClubJoinRequestResponseNotification(
                  userId: touristId,
                  actorId: ownerId,
                  clubId: clubId,
                  clubName: club.Name,
                  accepted: false
              );
        }

    }
}
