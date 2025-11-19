using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aiursoft.Tracer.MySql.Migrations
{
    /// <inheritdoc />
    public partial class AddDisplayName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Unknown")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AspNetUsers");
        }
    }
}
