namespace SportReservations.Api.Models;

public class Reservation
{
    public int Id { get; set; }
    public int FieldId { get; set; }
    public Field Field { get; set; } = null!;
    public string PlayerName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Notes { get; set; }
}
