INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "StartTime", "StartLatitude", "StartLongitude", "Status", "EndTime", "LastActivity")
VALUES (-1, -21, -2, '2024-01-15 10:00:00+00', 45.2671, 19.8335, 0, NULL, '2024-01-15 10:00:00+00');

INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "StartTime", "StartLatitude", "StartLongitude", "Status", "EndTime", "LastActivity")
VALUES (-2, -21, -2, '2024-01-16 14:30:00+00', 45.2671, 19.8335, 1, '2024-01-16 16:45:00+00', '2024-01-16 16:45:00+00');

INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "StartTime", "StartLatitude", "StartLongitude", "Status", "EndTime", "LastActivity")
VALUES (-3, -22, -1, '2024-01-17 09:15:00+00', 45.2671, 19.8335, 2, '2024-01-17 10:30:00+00', '2024-01-17 10:30:00+00');

INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "StartTime", "StartLatitude", "StartLongitude", "Status", "EndTime", "LastActivity")
VALUES (-4, -22, -3, '2024-01-18 11:20:00+00', 45.2671, 19.8335, 0, NULL, '2024-01-18 11:20:00+00');



--dodato za TourReview

INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "StartTime", "StartLatitude", "StartLongitude", "Status", "EndTime", "LastActivity")
VALUES (-10, -1, -1, '2024-01-15 10:00:00+00', 45.2671, 19.8335, 1, '2025-12-09 15:00:00+00', '2025-12-09 15:00:00+00');

INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "StartTime", "StartLatitude", "StartLongitude", "Status", "EndTime", "LastActivity")
VALUES (-11, -2, -2, '2024-01-16 10:00:00+00', 45.2671, 19.8335, 0, NULL, '2025-12-10 14:30:00+00');

INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "StartTime", "StartLatitude", "StartLongitude", "Status", "EndTime", "LastActivity")
VALUES (-12, -2, -1, '2024-01-17 10:00:00+00', 45.2671, 19.8335, 0, NULL, '2025-12-08 10:15:00+00');

INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "StartTime", "StartLatitude", "StartLongitude", "Status", "EndTime", "LastActivity")
VALUES (-14, -1, -2, '2024-01-20 10:00:00+00', 45.2671, 19.8335, 1, '2025-12-08 13:00:00+00', '2025-12-08 13:00:00+00');

INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "StartTime", "StartLatitude", "StartLongitude", "Status", "EndTime", "LastActivity")
VALUES (-15, -1, -2, '2024-01-25 10:00:00+00', 45.2671, 19.8335, 0, NULL, '2025-12-10 16:00:00+00');

-- CompletedTourPoint

INSERT INTO tours."CompletedTourPoint"("TourPointId", "CompletedAt", "TourExecutionId")
VALUES 
(-101, '2024-01-15 10:10:00+00', -10),
(-102, '2024-01-15 10:20:00+00', -10),
(-103, '2024-01-15 10:30:00+00', -10),
(-104, '2024-01-15 10:40:00+00', -10),
(-105, '2024-01-15 10:50:00+00', -10),
(-106, '2024-01-15 11:00:00+00', -10),
(-107, '2024-01-15 11:10:00+00', -10),
(-108, '2024-01-15 11:20:00+00', -10),
(-109, '2024-01-15 11:30:00+00', -10),
(-110, '2024-01-15 11:40:00+00', -10);

INSERT INTO tours."CompletedTourPoint"("TourPointId", "CompletedAt", "TourExecutionId")
VALUES 
(-200, '2024-01-16 10:10:00+00', -11),
(-201, '2024-01-16 10:20:00+00', -11),
(-202, '2024-01-16 10:30:00+00', -11),
(-203, '2024-01-16 10:40:00+00', -11),
(-204, '2024-01-16 10:50:00+00', -11),
(-205, '2024-01-16 11:00:00+00', -11),
(-206, '2024-01-16 11:10:00+00', -11),
(-207, '2024-01-16 11:20:00+00', -11),
(-208, '2024-01-16 11:30:00+00', -11),
(-209, '2024-01-16 11:40:00+00', -11),
(-210, '2024-01-16 11:50:00+00', -11),
(-211, '2024-01-16 12:00:00+00', -11),
(-212, '2024-01-16 12:10:00+00', -11),
(-213, '2024-01-16 12:20:00+00', -11),
(-214, '2024-01-16 12:30:00+00', -11),
(-215, '2024-01-16 12:40:00+00', -11),
(-216, '2024-01-16 12:50:00+00', -11);

INSERT INTO tours."CompletedTourPoint"("TourPointId", "CompletedAt", "TourExecutionId")
VALUES 
(-101, '2024-01-17 10:10:00+00', -12),
(-102, '2024-01-17 10:20:00+00', -12),
(-103, '2024-01-17 10:30:00+00', -12),
(-104, '2024-01-17 10:40:00+00', -12),
(-105, '2024-01-17 10:50:00+00', -12);

INSERT INTO tours."CompletedTourPoint"("TourPointId", "CompletedAt", "TourExecutionId")
VALUES 
(-200, '2024-01-20 10:10:00+00', -14),
(-201, '2024-01-20 10:20:00+00', -14),
(-202, '2024-01-20 10:30:00+00', -14),
(-203, '2024-01-20 10:40:00+00', -14),
(-204, '2024-01-20 10:50:00+00', -14),
(-205, '2024-01-20 11:00:00+00', -14),
(-206, '2024-01-20 11:10:00+00', -14),
(-207, '2024-01-20 11:20:00+00', -14),
(-208, '2024-01-20 11:30:00+00', -14),
(-209, '2024-01-20 11:40:00+00', -14),
(-210, '2024-01-20 11:50:00+00', -14),
(-211, '2024-01-20 12:00:00+00', -14),
(-212, '2024-01-20 12:10:00+00', -14),
(-213, '2024-01-20 12:20:00+00', -14),
(-214, '2024-01-20 12:30:00+00', -14),
(-215, '2024-01-20 12:40:00+00', -14),
(-216, '2024-01-20 12:50:00+00', -14);

INSERT INTO tours."CompletedTourPoint"("TourPointId", "CompletedAt", "TourExecutionId")
VALUES 
(-200, '2024-01-25 10:10:00+00', -15),
(-201, '2024-01-25 10:20:00+00', -15),
(-202, '2024-01-25 10:30:00+00', -15),
(-203, '2024-01-25 10:40:00+00', -15);