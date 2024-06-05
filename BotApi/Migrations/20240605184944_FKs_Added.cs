using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotApi.Migrations
{
    /// <inheritdoc />
    public partial class FKs_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WorkerDisciplines_DisciplineID",
                table: "WorkerDisciplines",
                column: "DisciplineID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerDisciplines_Disciplines_DisciplineID",
                table: "WorkerDisciplines",
                column: "DisciplineID",
                principalTable: "Disciplines",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerDisciplines_Workers_WorkerID",
                table: "WorkerDisciplines",
                column: "WorkerID",
                principalTable: "Workers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkerDisciplines_Disciplines_DisciplineID",
                table: "WorkerDisciplines");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerDisciplines_Workers_WorkerID",
                table: "WorkerDisciplines");

            migrationBuilder.DropIndex(
                name: "IX_WorkerDisciplines_DisciplineID",
                table: "WorkerDisciplines");
        }
    }
}
