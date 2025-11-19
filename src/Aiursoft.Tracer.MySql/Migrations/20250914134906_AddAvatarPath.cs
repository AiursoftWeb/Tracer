using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aiursoft.Tracer.MySql.Migrations
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
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
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
