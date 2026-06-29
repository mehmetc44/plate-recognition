using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlakaTanima.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnprEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnprEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Plate = table.Column<string>(type: "text", nullable: false),
                    CameraName = table.Column<string>(type: "text", nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Confidence = table.Column<double>(type: "double precision", nullable: false),
                    VehicleType = table.Column<string>(type: "text", nullable: false),
                    VehicleColor = table.Column<string>(type: "text", nullable: false),
                    VehicleBrand = table.Column<string>(type: "text", nullable: false),
                    Direction = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    PlateImagePath = table.Column<string>(type: "text", nullable: true),
                    VehicleImagePath = table.Column<string>(type: "text", nullable: true),
                    FullImagePath = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnprEvents", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnprEvents");
        }
    }
}
