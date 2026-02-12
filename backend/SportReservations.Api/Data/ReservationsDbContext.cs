using Microsoft.EntityFrameworkCore;
using SportReservations.Api.Models;

namespace SportReservations.Api.Data;

public class ReservationsDbContext(DbContextOptions<ReservationsDbContext> options) : DbContext(options)
{
    public DbSet<Field> Fields => Set<Field>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Field>(entity =>
        {
            entity.Property(f => f.Name).HasMaxLength(120).IsRequired();
            entity.Property(f => f.SurfaceType).HasMaxLength(80).IsRequired();
            entity.HasIndex(f => f.Name).IsUnique();
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable(table =>
            {
                table.HasCheckConstraint("CK_Reservations_StartBeforeEnd", "\"StartTime\" < \"EndTime\"");
            });

            entity.Property(r => r.PlayerName).HasMaxLength(120).IsRequired();
            entity.Property(r => r.Notes).HasMaxLength(500);

            entity.HasOne(r => r.Field)
                .WithMany(f => f.Reservations)
                .HasForeignKey(r => r.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(r => new { r.FieldId, r.StartTime, r.EndTime });
        });
    }
}
