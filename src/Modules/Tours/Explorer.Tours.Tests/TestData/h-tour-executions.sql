INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "StartTime", "StartLatitude", "StartLongitude", "Status", "EndTime", "LastActivity")
VALUES (-10, -1, -1, CURRENT_TIMESTAMP - INTERVAL '10 days', 45.2671, 19.8335, 1, 
        CURRENT_TIMESTAMP - INTERVAL '2 days', CURRENT_TIMESTAMP - INTERVAL '2 days');


INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "StartTime", "StartLatitude", "StartLongitude", "Status", "EndTime", "LastActivity")
VALUES (-11, -2, -2, CURRENT_TIMESTAMP - INTERVAL '5 days', 45.2671, 19.8335, 0, NULL, 
        CURRENT_TIMESTAMP - INTERVAL '1 day');


INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "StartTime", "StartLatitude", "StartLongitude", "Status", "EndTime", "LastActivity")
VALUES (-12, -2, -1, CURRENT_TIMESTAMP - INTERVAL '6 days', 45.2671, 19.8335, 0, NULL, 
        CURRENT_TIMESTAMP - INTERVAL '3 days');


INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "StartTime", "StartLatitude", "StartLongitude", "Status", "EndTime", "LastActivity")
VALUES (-14, -1, -2, CURRENT_TIMESTAMP - INTERVAL '8 days', 45.2671, 19.8335, 1, 
        CURRENT_TIMESTAMP - INTERVAL '2 days', CURRENT_TIMESTAMP - INTERVAL '2 days');


INSERT INTO tours."TourExecutions"(
    "Id", "TouristId", "TourId", "StartTime", "StartLatitude", "StartLongitude", "Status", "EndTime", "LastActivity")
VALUES (-15, -1, -2, CURRENT_TIMESTAMP - INTERVAL '4 days', 45.2671, 19.8335, 0, NULL, 
        CURRENT_TIMESTAMP - INTERVAL '1 day');


INSERT INTO tours."CompletedTourPoint"("TourPointId", "CompletedAt", "TourExecutionId")
VALUES 
(-101, CURRENT_TIMESTAMP - INTERVAL '2 days 1 hour 50 minutes', -10),
(-102, CURRENT_TIMESTAMP - INTERVAL '2 days 1 hour 40 minutes', -10),
(-103, CURRENT_TIMESTAMP - INTERVAL '2 days 1 hour 30 minutes', -10),
(-104, CURRENT_TIMESTAMP - INTERVAL '2 days 1 hour 20 minutes', -10),
(-105, CURRENT_TIMESTAMP - INTERVAL '2 days 1 hour 10 minutes', -10),
(-106, CURRENT_TIMESTAMP - INTERVAL '2 days 1 hour', -10),
(-107, CURRENT_TIMESTAMP - INTERVAL '2 days 50 minutes', -10),
(-108, CURRENT_TIMESTAMP - INTERVAL '2 days 40 minutes', -10),
(-109, CURRENT_TIMESTAMP - INTERVAL '2 days 30 minutes', -10),
(-110, CURRENT_TIMESTAMP - INTERVAL '2 days 20 minutes', -10);


INSERT INTO tours."CompletedTourPoint"("TourPointId", "CompletedAt", "TourExecutionId")
VALUES 
(-200, CURRENT_TIMESTAMP - INTERVAL '1 day 1 hour 50 minutes', -11),
(-201, CURRENT_TIMESTAMP - INTERVAL '1 day 1 hour 40 minutes', -11),
(-202, CURRENT_TIMESTAMP - INTERVAL '1 day 1 hour 30 minutes', -11),
(-203, CURRENT_TIMESTAMP - INTERVAL '1 day 1 hour 20 minutes', -11),
(-204, CURRENT_TIMESTAMP - INTERVAL '1 day 1 hour 10 minutes', -11),
(-205, CURRENT_TIMESTAMP - INTERVAL '1 day 1 hour', -11),
(-206, CURRENT_TIMESTAMP - INTERVAL '1 day 50 minutes', -11),
(-207, CURRENT_TIMESTAMP - INTERVAL '1 day 40 minutes', -11),
(-208, CURRENT_TIMESTAMP - INTERVAL '1 day 30 minutes', -11),
(-209, CURRENT_TIMESTAMP - INTERVAL '1 day 20 minutes', -11),
(-210, CURRENT_TIMESTAMP - INTERVAL '1 day 10 minutes', -11),
(-211, CURRENT_TIMESTAMP - INTERVAL '1 day', -11),
(-212, CURRENT_TIMESTAMP - INTERVAL '1 day' + INTERVAL '10 minutes', -11),
(-213, CURRENT_TIMESTAMP - INTERVAL '1 day' + INTERVAL '20 minutes', -11),
(-214, CURRENT_TIMESTAMP - INTERVAL '1 day' + INTERVAL '30 minutes', -11),
(-215, CURRENT_TIMESTAMP - INTERVAL '1 day' + INTERVAL '40 minutes', -11),
(-216, CURRENT_TIMESTAMP - INTERVAL '1 day' + INTERVAL '50 minutes', -11);


INSERT INTO tours."CompletedTourPoint"("TourPointId", "CompletedAt", "TourExecutionId")
VALUES 
(-101, CURRENT_TIMESTAMP - INTERVAL '3 days 1 hour 50 minutes', -12),
(-102, CURRENT_TIMESTAMP - INTERVAL '3 days 1 hour 40 minutes', -12),
(-103, CURRENT_TIMESTAMP - INTERVAL '3 days 1 hour 30 minutes', -12),
(-104, CURRENT_TIMESTAMP - INTERVAL '3 days 1 hour 20 minutes', -12),
(-105, CURRENT_TIMESTAMP - INTERVAL '3 days 1 hour 10 minutes', -12);


INSERT INTO tours."CompletedTourPoint"("TourPointId", "CompletedAt", "TourExecutionId")
VALUES 
(-200, CURRENT_TIMESTAMP - INTERVAL '2 days 1 hour 50 minutes', -14),
(-201, CURRENT_TIMESTAMP - INTERVAL '2 days 1 hour 40 minutes', -14),
(-202, CURRENT_TIMESTAMP - INTERVAL '2 days 1 hour 30 minutes', -14),
(-203, CURRENT_TIMESTAMP - INTERVAL '2 days 1 hour 20 minutes', -14),
(-204, CURRENT_TIMESTAMP - INTERVAL '2 days 1 hour 10 minutes', -14),
(-205, CURRENT_TIMESTAMP - INTERVAL '2 days 1 hour', -14),
(-206, CURRENT_TIMESTAMP - INTERVAL '2 days 50 minutes', -14),
(-207, CURRENT_TIMESTAMP - INTERVAL '2 days 40 minutes', -14),
(-208, CURRENT_TIMESTAMP - INTERVAL '2 days 30 minutes', -14),
(-209, CURRENT_TIMESTAMP - INTERVAL '2 days 20 minutes', -14),
(-210, CURRENT_TIMESTAMP - INTERVAL '2 days 10 minutes', -14),
(-211, CURRENT_TIMESTAMP - INTERVAL '2 days', -14),
(-212, CURRENT_TIMESTAMP - INTERVAL '2 days' + INTERVAL '10 minutes', -14),
(-213, CURRENT_TIMESTAMP - INTERVAL '2 days' + INTERVAL '20 minutes', -14),
(-214, CURRENT_TIMESTAMP - INTERVAL '2 days' + INTERVAL '30 minutes', -14),
(-215, CURRENT_TIMESTAMP - INTERVAL '2 days' + INTERVAL '40 minutes', -14),
(-216, CURRENT_TIMESTAMP - INTERVAL '2 days' + INTERVAL '50 minutes', -14);


INSERT INTO tours."CompletedTourPoint"("TourPointId", "CompletedAt", "TourExecutionId")
VALUES 
(-200, CURRENT_TIMESTAMP - INTERVAL '1 day 1 hour 50 minutes', -15),
(-201, CURRENT_TIMESTAMP - INTERVAL '1 day 1 hour 40 minutes', -15),
(-202, CURRENT_TIMESTAMP - INTERVAL '1 day 1 hour 30 minutes', -15),
(-203, CURRENT_TIMESTAMP - INTERVAL '1 day 1 hour 20 minutes', -15);