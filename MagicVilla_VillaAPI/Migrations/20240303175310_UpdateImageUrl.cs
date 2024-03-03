using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVilla_VillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ImageUrl" },
                values: new object[] { new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://placehold.co/600x401" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ImageUrl" },
                values: new object[] { new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://placehold.co/600x402" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ImageUrl" },
                values: new object[] { new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://placehold.co/600x403" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ImageUrl" },
                values: new object[] { new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://placehold.co/600x404" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ImageUrl" },
                values: new object[] { new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://placehold.co/600x405" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ImageUrl" },
                values: new object[] { new DateTime(2024, 3, 1, 12, 46, 23, 673, DateTimeKind.Local).AddTicks(6679), "https://dotnetmastery.com/bluevillaimages/villa3.jpg" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ImageUrl" },
                values: new object[] { new DateTime(2024, 3, 1, 12, 46, 23, 673, DateTimeKind.Local).AddTicks(6694), "https://dotnetmastery.com/bluevillaimages/villa1.jpg" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ImageUrl" },
                values: new object[] { new DateTime(2024, 3, 1, 12, 46, 23, 673, DateTimeKind.Local).AddTicks(6698), "https://dotnetmastery.com/bluevillaimages/villa4.jpg" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ImageUrl" },
                values: new object[] { new DateTime(2024, 3, 1, 12, 46, 23, 673, DateTimeKind.Local).AddTicks(6700), "https://dotnetmastery.com/bluevillaimages/villa5.jpg" });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ImageUrl" },
                values: new object[] { new DateTime(2024, 3, 1, 12, 46, 23, 673, DateTimeKind.Local).AddTicks(6703), "https://dotnetmastery.com/bluevillaimages/villa2.jpg" });
        }
    }
}
