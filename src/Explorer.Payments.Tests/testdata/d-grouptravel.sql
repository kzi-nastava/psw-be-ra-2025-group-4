INSERT INTO payments."GroupTravelRequests"(
    "Id", "OrganizerId", "TourId", "TourName", "PricePerPerson", "Status", "CreatedAt", "CompletedAt")
VALUES (-1, -1, -1, 'Test Tour 1', 100.00, 0, CURRENT_TIMESTAMP, NULL);

INSERT INTO payments."GroupTravelParticipants"(
    "Id", "GroupTravelRequestId", "TouristId", "Status", "RespondedAt")
VALUES (-1, -1, -2, 0, NULL);

INSERT INTO payments."GroupTravelRequests"(
    "Id", "OrganizerId", "TourId", "TourName", "PricePerPerson", "Status", "CreatedAt", "CompletedAt")
VALUES (-2, -1, -2, 'Test Tour 2', 150.00, 1, CURRENT_TIMESTAMP, NULL);

INSERT INTO payments."GroupTravelParticipants"(
    "Id", "GroupTravelRequestId", "TouristId", "Status", "RespondedAt")
VALUES (-2, -2, -2, 1, CURRENT_TIMESTAMP);

INSERT INTO payments."GroupTravelParticipants"(
    "Id", "GroupTravelRequestId", "TouristId", "Status", "RespondedAt")
VALUES (-3, -2, -3, 1, CURRENT_TIMESTAMP);
