using SportReservations.Api.Models;

namespace SportReservations.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(ReservationsDbContext db)
    {
        if (db.Fields.Any())
        {
            return;
        }

        db.Fields.AddRange(
            new Field { Name = "Main Football Arena", SurfaceType = "Natural grass", Capacity = 22 },
            new Field { Name = "Futsal Court A", SurfaceType = "Indoor synthetic", Capacity = 10 },
            new Field { Name = "Training Pitch", SurfaceType = "Artificial turf", Capacity = 14 }
        );

        await db.SaveChangesAsync();
    }
}
