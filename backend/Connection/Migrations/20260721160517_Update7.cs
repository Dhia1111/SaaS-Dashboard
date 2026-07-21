using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connection.Migrations
{
    /// <inheritdoc />
    public partial class Update7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlatformSubscriptions_TenantPlanPricingOptionId",
                table: "PlatformSubscriptions");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformSubscriptions_TenantPlanPricingOptionId",
                table: "PlatformSubscriptions",
                column: "TenantPlanPricingOptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlatformSubscriptions_TenantPlanPricingOptionId",
                table: "PlatformSubscriptions");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformSubscriptions_TenantPlanPricingOptionId",
                table: "PlatformSubscriptions",
                column: "TenantPlanPricingOptionId",
                unique: true);
        }
    }
}
