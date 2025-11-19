using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aiursoft.Tracer.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarRelativePath",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarRelativePath",
                table: "AspNetUsers");
        }
    }
}
