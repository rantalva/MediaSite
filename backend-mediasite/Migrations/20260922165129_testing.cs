using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaSite_backend.Migrations
{
    /// <inheritdoc />
    public partial class testing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "NewsletterSubscribers");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "NewsletterSubscribers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "NewsletterSubscribers");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "NewsletterSubscribers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
