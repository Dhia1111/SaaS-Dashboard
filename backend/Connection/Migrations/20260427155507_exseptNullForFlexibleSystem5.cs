using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connection.Migrations
{
    /// <inheritdoc />
    public partial class exseptNullForFlexibleSystem5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataKey",
                table: "Persons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DataKey",
                table: "Persons",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
