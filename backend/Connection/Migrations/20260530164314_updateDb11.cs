using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connection.Migrations
{
    /// <inheritdoc />
    public partial class updateDb11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingInterval",
                table: "TenantPlans");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "TenantPlans");

            migrationBuilder.DropColumn(
                name: "StripePriceId",
                table: "TenantPlans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillingInterval",
                table: "TenantPlans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "TenantPlans",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "StripePriceId",
                table: "TenantPlans",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
