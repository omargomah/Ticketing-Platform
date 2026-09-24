using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTheRolesOfUserInDataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("47248ad3-f771-4e70-bb3d-b911502de992"), "4963c7c6-172b-4c07-b590-b0add57e4802", "Admin", "ADMIN" },
                    { new Guid("69d005f7-a503-4a0f-b7f0-90d64f19c445"), "a3f88cf3-9694-4b9b-99cc-f23cecf92e0c", "Attendee", "ATTENDEE" },
                    { new Guid("74ee2599-2a9e-42c5-8071-16cef0080b77"), "13914f17-c84e-418a-9109-fa2d5b96ed82", "Organizer", "ORGANIZER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("47248ad3-f771-4e70-bb3d-b911502de992"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("69d005f7-a503-4a0f-b7f0-90d64f19c445"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("74ee2599-2a9e-42c5-8071-16cef0080b77"));
        }
    }
}
