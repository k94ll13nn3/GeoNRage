using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoNRage.Server.Migrations
{
    /// <inheritdoc />
    public partial class RevertUpdateOnChallenge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Players_CreatorId",
                table: "Challenges");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Challenges",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "Challenges",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Players_CreatorId",
                table: "Challenges",
                column: "CreatorId",
                principalTable: "Players",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Players_CreatorId",
                table: "Challenges");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Challenges",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Challenges",
                keyColumn: "CreatorId",
                keyValue: null,
                column: "CreatorId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "Challenges",
                type: "varchar(255)",
                nullable: false,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Players_CreatorId",
                table: "Challenges",
                column: "CreatorId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
