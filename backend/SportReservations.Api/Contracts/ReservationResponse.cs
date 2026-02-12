namespace SportReservations.Api.Contracts;

public record ReservationResponse(
    int Id,
    int FieldId,
    string FieldName,
    string PlayerName,
    DateTime StartTime,
    DateTime EndTime,
    string? Notes);
