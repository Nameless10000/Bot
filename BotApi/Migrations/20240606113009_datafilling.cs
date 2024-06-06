using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotApi.Migrations
{
    /// <inheritdoc />
    public partial class datafilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Appointments",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Disciplines",
                columns: new[] { "ID", "Name" },
                values: new object[] { 1, "Заработок на росте криптовалют в порно-играх" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "ID", "UserName" },
                values: new object[] { 659615698L, "Quazzik" });

            migrationBuilder.InsertData(
                table: "Workers",
                columns: new[] { "ID", "UserName" },
                values: new object[] { 806499592L, "znya05" });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "DisciplineID", "UserID", "WorkerID", "Description", "Longevity", "Price", "StartsAt" },
                values: new object[] { 1, 659615698L, 806499592L, "По, купону бесплатное первое занятие", new TimeSpan(0, 1, 0, 0, 0), 1m, new DateTime(2024, 6, 6, 16, 30, 8, 637, DateTimeKind.Local).AddTicks(161) });

            migrationBuilder.InsertData(
                table: "WorkerDisciplines",
                columns: new[] { "DisciplineID", "WorkerID" },
                values: new object[] { 1, 806499592L });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumns: new[] { "DisciplineID", "UserID", "WorkerID" },
                keyValues: new object[] { 1, 659615698L, 806499592L });

            migrationBuilder.DeleteData(
                table: "WorkerDisciplines",
                keyColumns: new[] { "DisciplineID", "WorkerID" },
                keyValues: new object[] { 1, 806499592L });

            migrationBuilder.DeleteData(
                table: "Disciplines",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "ID",
                keyValue: 659615698L);

            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "ID",
                keyValue: 806499592L);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Description",
                keyValue: null,
                column: "Description",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Appointments",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
