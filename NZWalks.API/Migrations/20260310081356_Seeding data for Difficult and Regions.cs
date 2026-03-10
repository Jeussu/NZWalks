using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NZWalks.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedingdataforDifficultandRegions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Difficulties",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-7890-1234-56789abcdef0"), "Easy" },
                    { new Guid("b1c2d3e4-f5a6-7890-1234-56789abcdef0"), "Medium" },
                    { new Guid("c1d2e3f4-a5b6-7890-1234-56789abcdef0"), "Hard" }
                });

            migrationBuilder.InsertData(
                table: "Regions",
                columns: new[] { "Id", "Code", "Name", "RegionImageUrl" },
                values: new object[,]
                {
                    { new Guid("d1e2f3a4-b5c6-7890-1234-56789abcdef0"), "AKL", "Auckland", "https://example.com/images/auckland.jpg" },
                    { new Guid("e1f2a3b4-c5d6-7890-1234-56789abcdef0"), "WLG", "Wellington", "https://example.com/images/wellington.jpg" },
                    { new Guid("f1a2b3c4-d5e6-7890-1234-56789abcdef0"), "CHC", "Christchurch", "https://example.com/images/christchurch.jpg" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1234-56789abcdef0"));

            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: new Guid("b1c2d3e4-f5a6-7890-1234-56789abcdef0"));

            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: new Guid("c1d2e3f4-a5b6-7890-1234-56789abcdef0"));

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("d1e2f3a4-b5c6-7890-1234-56789abcdef0"));

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("e1f2a3b4-c5d6-7890-1234-56789abcdef0"));

            migrationBuilder.DeleteData(
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("f1a2b3c4-d5e6-7890-1234-56789abcdef0"));
        }
    }
}
