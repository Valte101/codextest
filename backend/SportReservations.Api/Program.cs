using Microsoft.EntityFrameworkCore;
using System.Data;
using Npgsql;
using SportReservations.Api.Contracts;
using SportReservations.Api.Data;
using SportReservations.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins(builder.Configuration["FrontendOrigin"] ?? "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ReservationsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReservationsDbContext>();
    db.Database.Migrate();
    await SeedData.InitializeAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("frontend");

app.MapGet("/api/fields", async (ReservationsDbContext db) =>
{
    var fields = await db.Fields.OrderBy(f => f.Name).ToListAsync();
    return Results.Ok(fields);
});

app.MapGet("/api/reservations", async (ReservationsDbContext db) =>
{
    var reservations = await db.Reservations
        .AsNoTracking()
        .Include(r => r.Field)
        .OrderBy(r => r.StartTime)
        .Select(r => new ReservationResponse(
            r.Id,
            r.FieldId,
            r.Field.Name,
            r.PlayerName,
            r.StartTime,
            r.EndTime,
            r.Notes))
        .ToListAsync();

    return Results.Ok(reservations);
});

app.MapPost("/api/reservations", async (CreateReservationRequest request, ReservationsDbContext db) =>
{
    if (request.EndTime <= request.StartTime)
    {
        return Results.BadRequest(new { error = "End time must be after start time." });
    }

    var fieldExists = await db.Fields.AnyAsync(f => f.Id == request.FieldId);
    if (!fieldExists)
    {
        return Results.NotFound(new { error = "Field was not found." });
    }

    var reservation = new Reservation
    {
        FieldId = request.FieldId,
        PlayerName = request.PlayerName.Trim(),
        StartTime = request.StartTime,
        EndTime = request.EndTime,
        Notes = request.Notes?.Trim()
    };

    try
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var overlapExists = await db.Reservations.AnyAsync(r =>
            r.FieldId == request.FieldId &&
            request.StartTime < r.EndTime &&
            request.EndTime > r.StartTime);

        if (overlapExists)
        {
            return Results.Conflict(new { error = "This field is already reserved for that time range." });
        }

        db.Reservations.Add(reservation);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    catch (DbUpdateException ex) when (ex.InnerException is PostgresException
    {
        SqlState: PostgresErrorCodes.ExclusionViolation or PostgresErrorCodes.SerializationFailure
    })
    {
        return Results.Conflict(new { error = "This field is already reserved for that time range." });
    }

    var fieldName = await db.Fields
        .Where(f => f.Id == request.FieldId)
        .Select(f => f.Name)
        .SingleAsync();

    return Results.Created($"/api/reservations/{reservation.Id}", new ReservationResponse(
        reservation.Id,
        reservation.FieldId,
        fieldName,
        reservation.PlayerName,
        reservation.StartTime,
        reservation.EndTime,
        reservation.Notes));
});

app.Run();
