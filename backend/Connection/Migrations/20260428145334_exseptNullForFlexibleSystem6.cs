using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connection.Migrations
{
    /// <inheritdoc />
    public partial class exseptNullForFlexibleSystem6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataKey",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "DataKey",
                table: "UserPayments");

            migrationBuilder.DropColumn(
                name: "DataKey",
                table: "TenantPlans");

            migrationBuilder.DropColumn(
                name: "DataKey",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "DataKey",
                table: "PlatformSubscriptions");

            migrationBuilder.DropColumn(
                name: "DataKey",
                table: "PlatformPayments");

            migrationBuilder.RenameColumn(
                name: "DataKey",
                table: "Users",
                newName: "TenantId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Tenants",
                newName: "TenantId");

            migrationBuilder.RenameColumn(
                name: "DataKey",
                table: "PlatformPlans",
                newName: "TenantId");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Persons",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Persons");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "Users",
                newName: "DataKey");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "Tenants",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "PlatformPlans",
                newName: "DataKey");

            migrationBuilder.AddColumn<int>(
                name: "DataKey",
                table: "UserSubscriptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DataKey",
                table: "UserPayments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DataKey",
                table: "TenantPlans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DataKey",
                table: "Sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DataKey",
                table: "PlatformSubscriptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DataKey",
                table: "PlatformPayments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
