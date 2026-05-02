using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connection.Migrations
{
    /// <inheritdoc />
    public partial class Delete_DataKeyAndTenantIdFromEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataKey",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Emails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DataKey",
                table: "Emails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Emails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
