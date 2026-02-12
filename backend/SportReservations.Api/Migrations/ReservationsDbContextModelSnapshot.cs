using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SportReservations.Api.Data;

#nullable disable

namespace SportReservations.Api.Migrations;

[DbContext(typeof(ReservationsDbContext))]
partial class ReservationsDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.11")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("SportReservations.Api.Models.Field", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<int>("Capacity")
                    .HasColumnType("integer");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(120)
                    .HasColumnType("character varying(120)");

                b.Property<string>("SurfaceType")
                    .IsRequired()
                    .HasMaxLength(80)
                    .HasColumnType("character varying(80)");

                b.HasKey("Id");

                b.HasIndex("Name")
                    .IsUnique();

                b.ToTable("Fields");
            });

        modelBuilder.Entity("SportReservations.Api.Models.Reservation", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<DateTime>("EndTime")
                    .HasColumnType("timestamp with time zone");

                b.Property<int>("FieldId")
                    .HasColumnType("integer");

                b.Property<string>("Notes")
                    .HasMaxLength(500)
                    .HasColumnType("character varying(500)");

                b.Property<string>("PlayerName")
                    .IsRequired()
                    .HasMaxLength(120)
                    .HasColumnType("character varying(120)");

                b.Property<DateTime>("StartTime")
                    .HasColumnType("timestamp with time zone");

                b.HasKey("Id");

                b.HasIndex("FieldId", "StartTime", "EndTime");

                b.ToTable("Reservations");
            });

        modelBuilder.Entity("SportReservations.Api.Models.Reservation", b =>
            {
                b.HasOne("SportReservations.Api.Models.Field", "Field")
                    .WithMany("Reservations")
                    .HasForeignKey("FieldId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Field");
            });

        modelBuilder.Entity("SportReservations.Api.Models.Field", b =>
            {
                b.Navigation("Reservations");
            });
#pragma warning restore 612, 618
    }
}
