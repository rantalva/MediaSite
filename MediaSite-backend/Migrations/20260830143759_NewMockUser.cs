using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaSite_backend.Migrations
{
    /// <inheritdoc />
    public partial class NewMockUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ApplicationUser",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("df3369fe-d3fa-41fb-b76a-05b4f64d042d"), 0, "22222222-2222-2222-2222-222222222222", "alvari.rantapelkonen@gmail.com", true, "Alvari", "Rantapelkonen", false, null, "ALVARI.RANTAPELKONEN@GMAIL.COM", "ALVARI.RANTAPELKONEN@GMAIL.COM", "Gt9Yc4AiIvmsC1QQbe2RZsCIqvoYlst2xbz0Fs8aHnw=", null, false, "11111111-1111-1111-1111-111111111111", false, "alvari.rantapelkonen@gmail.com" });

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "AuthorId",
                value: new Guid("df3369fe-d3fa-41fb-b76a-05b4f64d042d"));

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "AuthorId",
                value: new Guid("df3369fe-d3fa-41fb-b76a-05b4f64d042d"));

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                column: "AuthorId",
                value: new Guid("df3369fe-d3fa-41fb-b76a-05b4f64d042d"));

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                column: "AuthorId",
                value: new Guid("df3369fe-d3fa-41fb-b76a-05b4f64d042d"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ApplicationUser",
                keyColumn: "Id",
                keyValue: new Guid("df3369fe-d3fa-41fb-b76a-05b4f64d042d"));

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                column: "AuthorId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                column: "AuthorId",
                value: null);
        }
    }
}
