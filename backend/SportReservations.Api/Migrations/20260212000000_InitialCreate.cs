using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SportReservations.Api.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Fields",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                SurfaceType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                Capacity = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Fields", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Reservations",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                FieldId = table.Column<int>(type: "integer", nullable: false),
                PlayerName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Reservations", x => x.Id);
                table.ForeignKey(
                    name: "FK_Reservations_Fields_FieldId",
                    column: x => x.FieldId,
                    principalTable: "Fields",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Fields_Name",
            table: "Fields",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Reservations_FieldId_StartTime_EndTime",
            table: "Reservations",
            columns: new[] { "FieldId", "StartTime", "EndTime" });

        migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");
        migrationBuilder.Sql(
            "ALTER TABLE \"Reservations\" " +
            "ADD CONSTRAINT \"CK_Reservations_StartBeforeEnd\" CHECK (\"StartTime\" < \"EndTime\");");

        migrationBuilder.Sql(
            "ALTER TABLE \"Reservations\" " +
            "ADD CONSTRAINT \"EX_Reservations_FieldId_TimeRange\" " +
            "EXCLUDE USING gist (\"FieldId\" WITH =, tstzrange(\"StartTime\", \"EndTime\", '[)') WITH &&);");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("ALTER TABLE \"Reservations\" DROP CONSTRAINT IF EXISTS \"EX_Reservations_FieldId_TimeRange\";");
        migrationBuilder.Sql("ALTER TABLE \"Reservations\" DROP CONSTRAINT IF EXISTS \"CK_Reservations_StartBeforeEnd\";");

        migrationBuilder.DropTable(
            name: "Reservations");

        migrationBuilder.DropTable(
            name: "Fields");
    }
}
