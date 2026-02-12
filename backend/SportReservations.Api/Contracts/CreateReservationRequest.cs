namespace SportReservations.Api.Contracts;

public record CreateReservationRequest(
    int FieldId,
    string PlayerName,
    DateTime StartTime,
    DateTime EndTime,
    string? Notes);
