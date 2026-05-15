using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlakaTanima.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LprEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Plate = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CameraName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Confidence = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LprEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LprImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LprImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LprImages_LprEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "LprEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LprRawEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    RawJson = table.Column<string>(type: "jsonb", nullable: false),
                    RawXml = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LprRawEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LprRawEvents_LprEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "LprEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LprImages_EventId",
                table: "LprImages",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_LprRawEvents_EventId",
                table: "LprRawEvents",
                column: "EventId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LprImages");

            migrationBuilder.DropTable(
                name: "LprRawEvents");

            migrationBuilder.DropTable(
                name: "LprEvents");
        }
    }
}
