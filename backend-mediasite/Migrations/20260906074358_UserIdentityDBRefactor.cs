using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MediaSite_backend.Migrations
{
    /// <inheritdoc />
    public partial class UserIdentityDBRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_AspNetUsers_AuthorId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Categories_CategoryId",
                table: "Articles");

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("df3369fe-d3fa-41fb-b76a-05b4f64d042d"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "Articles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ApplicationUserId",
                table: "Articles",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_AspNetUsers_ApplicationUserId",
                table: "Articles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_AspNetUsers_AuthorId",
                table: "Articles",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Categories_CategoryId",
                table: "Articles",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_AspNetUsers_ApplicationUserId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_AspNetUsers_AuthorId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Categories_CategoryId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ApplicationUserId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Articles");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("df3369fe-d3fa-41fb-b76a-05b4f64d042d"), 0, "22222222-2222-2222-2222-222222222222", "alvari.rantapelkonen@gmail.com", true, "Alvari", "Rantapelkonen", false, null, "ALVARI.RANTAPELKONEN@GMAIL.COM", "ALVARI.RANTAPELKONEN@GMAIL.COM", "Gt9Yc4AiIvmsC1QQbe2RZsCIqvoYlst2xbz0Fs8aHnw=", null, false, "11111111-1111-1111-1111-111111111111", false, "alvari.rantapelkonen@gmail.com" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Style" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Shopping" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Culture" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Sports" }
                });

            migrationBuilder.InsertData(
                table: "Articles",
                columns: new[] { "Id", "AuthorId", "CategoryId", "Content", "CreatedDate", "HeroImage", "LastEditDate", "Slug", "Title" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("df3369fe-d3fa-41fb-b76a-05b4f64d042d"), new Guid("11111111-1111-1111-1111-111111111111"), "Lorem ipsum dolor sit amet...", new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Utc), "../Uploads/Menswear+closet.jpg", null, "vaatekaapin-kulmakivien-opas-2026", "Vaatekaapin kulmakivien opas 2026" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("df3369fe-d3fa-41fb-b76a-05b4f64d042d"), new Guid("22222222-2222-2222-2222-222222222222"), "Lorem ipsum dolor sit amet...", new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Utc), "../Uploads/Menswear+closet.jpg", null, "black-fridayn-parhaimmat-ostosvinkit-miehille", "Black Fridayn parhaimmat ostosvinkit miehille" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("df3369fe-d3fa-41fb-b76a-05b4f64d042d"), new Guid("33333333-3333-3333-3333-333333333333"), "Lorem ipsum dolor sit amet...", new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Utc), "../Uploads/Menswear+closet.jpg", null, "hellride-2026-madness-valokuvissa", "Hellride 2026: Madness valokuvissa" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("df3369fe-d3fa-41fb-b76a-05b4f64d042d"), new Guid("44444444-4444-4444-4444-444444444444"), "Lorem ipsum dolor sit amet...", new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Utc), "../Uploads/Menswear+closet.jpg", null, "jokerit-palaa-liigaan-paakaupungin-derbyt-vuonna-2026", "Jokerit palaa liigaan: Pääkaupungin derbyt vuonna 2026" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_AspNetUsers_AuthorId",
                table: "Articles",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Categories_CategoryId",
                table: "Articles",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
