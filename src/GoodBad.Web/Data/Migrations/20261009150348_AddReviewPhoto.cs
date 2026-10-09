using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodBad.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Reviews",
                type: "varchar(600)",
                maxLength: 600,
                nullable: true,
                collation: "utf8mb4_unicode_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Reviews");
        }
    }
}
