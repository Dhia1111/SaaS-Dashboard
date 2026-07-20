using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connection.Migrations
{
    /// <inheritdoc />
    public partial class Update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantsFreePlans_TenantPlan_TenantPlanId1",
                table: "TenantsFreePlans");

            migrationBuilder.DropIndex(
                name: "IX_TenantsFreePlans_TenantPlanId1",
                table: "TenantsFreePlans");

            migrationBuilder.DropColumn(
                name: "TenantPlanId1",
                table: "TenantsFreePlans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantPlanId1",
                table: "TenantsFreePlans",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantsFreePlans_TenantPlanId1",
                table: "TenantsFreePlans",
                column: "TenantPlanId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantsFreePlans_TenantPlan_TenantPlanId1",
                table: "TenantsFreePlans",
                column: "TenantPlanId1",
                principalTable: "TenantPlan",
                principalColumn: "Id");
        }
    }
}
