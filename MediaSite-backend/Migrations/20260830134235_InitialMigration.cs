using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MediaSite_backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    HeroImage = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_ApplicationUser_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Articles_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

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
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, new Guid("11111111-1111-1111-1111-111111111111"), "Lorem ipsum dolor sit amet...", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "../Uploads/Menswear+closet.jpg", null, "vaatekaapin-kulmakivien-opas-2026", "Vaatekaapin kulmakivien opas 2026" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), null, new Guid("22222222-2222-2222-2222-222222222222"), "Lorem ipsum dolor sit amet...", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "../Uploads/Menswear+closet.jpg", null, "black-fridayn-parhaimmat-ostosvinkit-miehille", "Black Fridayn parhaimmat ostosvinkit miehille" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), null, new Guid("33333333-3333-3333-3333-333333333333"), "Lorem ipsum dolor sit amet...", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "../Uploads/Menswear+closet.jpg", null, "hellride-2026-madness-valokuvissa", "Hellride 2026: Madness valokuvissa" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), null, new Guid("44444444-4444-4444-4444-444444444444"), "Lorem ipsum dolor sit amet...", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "../Uploads/Menswear+closet.jpg", null, "jokerit-palaa-liigaan-paakaupungin-derbyt-vuonna-2026", "Jokerit palaa liigaan: Pääkaupungin derbyt vuonna 2026" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_AuthorId",
                table: "Articles",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CategoryId",
                table: "Articles",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "ApplicationUser");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
