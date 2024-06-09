using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotApi.Migrations
{
    /// <inheritdoc />
    public partial class appointment_PK_fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    WorkerID = table.Column<long>(type: "bigint", nullable: false),
                    DisciplineID = table.Column<int>(type: "int", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Longevity = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => new { x.UserID, x.WorkerID, x.DisciplineID, x.StartsAt });
                    table.ForeignKey(
                        name: "FK_Appointments_Disciplines_DisciplineID",
                        column: x => x.DisciplineID,
                        principalTable: "Disciplines",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Workers_WorkerID",
                        column: x => x.WorkerID,
                        principalTable: "Workers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "DisciplineID", "StartsAt", "UserID", "WorkerID", "Description", "Longevity", "Price" },
                values: new object[] { 1, new DateTime(2024, 6, 9, 17, 59, 56, 90, DateTimeKind.Local).AddTicks(6926), 659615698L, 806499592L, "По, купону бесплатное первое занятие", new TimeSpan(0, 1, 0, 0, 0), 1m });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DisciplineID",
                table: "Appointments",
                column: "DisciplineID");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_WorkerID",
                table: "Appointments",
                column: "WorkerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "WorkerDisciplines");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Disciplines");

            migrationBuilder.DropTable(
                name: "Workers");
        }
    }
}
