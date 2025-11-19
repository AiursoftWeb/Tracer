using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aiursoft.Tracer.MySql.Migrations
{
    /// <inheritdoc />
    public partial class AddThemeSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PreferDarkTheme",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferDarkTheme",
                table: "AspNetUsers");
        }
    }
}
