﻿// <auto-generated />
using Microsoft.EntityFrameworkCore.Migrations;

namespace GeoNRage.Server.Migrations
{
    public partial class AddChallengeCreator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Challenges",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_CreatorId",
                table: "Challenges",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Players_CreatorId",
                table: "Challenges",
                column: "CreatorId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Players_CreatorId",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_CreatorId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Challenges");
        }
    }
}