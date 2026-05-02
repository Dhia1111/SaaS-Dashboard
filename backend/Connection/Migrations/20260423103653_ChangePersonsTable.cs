using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connection.Migrations
{
    /// <inheritdoc />
    public partial class ChangePersonsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsEmailVeryfied",
                table: "Persons",
                newName: "IsVeryfied");

            migrationBuilder.AddColumn<int>(
                name: "Provider",
                table: "Persons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderId",
                table: "Persons",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Provider",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "Persons");

            migrationBuilder.RenameColumn(
                name: "IsVeryfied",
                table: "Persons",
                newName: "IsEmailVeryfied");
        }
    }
}
