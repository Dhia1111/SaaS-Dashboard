using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Connection.Migrations
{
    /// <inheritdoc />
    public partial class Update8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantPlan_Tenants_TenantId",
                table: "TenantPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantsFreePlans_TenantPlan_TenantPlanId",
                table: "TenantsFreePlans");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantsPlansBenifests_TenantPlan_TenantPlanId",
                table: "TenantsPlansBenifests");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantsPlansPermissions_TenantPlan_TenantPlanId",
                table: "TenantsPlansPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantsPricingOptions_TenantPlan_TenantPlanId",
                table: "TenantsPricingOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantPlan",
                table: "TenantPlan");

            migrationBuilder.RenameTable(
                name: "TenantPlan",
                newName: "TenantPlans");

            migrationBuilder.RenameIndex(
                name: "IX_TenantPlan_TenantId",
                table: "TenantPlans",
                newName: "IX_TenantPlans_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantPlan_Name_TenantId",
                table: "TenantPlans",
                newName: "IX_TenantPlans_Name_TenantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantPlans",
                table: "TenantPlans",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Domains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Domains_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Domains_TenantId",
                table: "Domains",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantPlans_Tenants_TenantId",
                table: "TenantPlans",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantsFreePlans_TenantPlans_TenantPlanId",
                table: "TenantsFreePlans",
                column: "TenantPlanId",
                principalTable: "TenantPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantsPlansBenifests_TenantPlans_TenantPlanId",
                table: "TenantsPlansBenifests",
                column: "TenantPlanId",
                principalTable: "TenantPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantsPlansPermissions_TenantPlans_TenantPlanId",
                table: "TenantsPlansPermissions",
                column: "TenantPlanId",
                principalTable: "TenantPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantsPricingOptions_TenantPlans_TenantPlanId",
                table: "TenantsPricingOptions",
                column: "TenantPlanId",
                principalTable: "TenantPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantPlans_Tenants_TenantId",
                table: "TenantPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantsFreePlans_TenantPlans_TenantPlanId",
                table: "TenantsFreePlans");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantsPlansBenifests_TenantPlans_TenantPlanId",
                table: "TenantsPlansBenifests");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantsPlansPermissions_TenantPlans_TenantPlanId",
                table: "TenantsPlansPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantsPricingOptions_TenantPlans_TenantPlanId",
                table: "TenantsPricingOptions");

            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantPlans",
                table: "TenantPlans");

            migrationBuilder.RenameTable(
                name: "TenantPlans",
                newName: "TenantPlan");

            migrationBuilder.RenameIndex(
                name: "IX_TenantPlans_TenantId",
                table: "TenantPlan",
                newName: "IX_TenantPlan_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantPlans_Name_TenantId",
                table: "TenantPlan",
                newName: "IX_TenantPlan_Name_TenantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantPlan",
                table: "TenantPlan",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantPlan_Tenants_TenantId",
                table: "TenantPlan",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantsFreePlans_TenantPlan_TenantPlanId",
                table: "TenantsFreePlans",
                column: "TenantPlanId",
                principalTable: "TenantPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantsPlansBenifests_TenantPlan_TenantPlanId",
                table: "TenantsPlansBenifests",
                column: "TenantPlanId",
                principalTable: "TenantPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantsPlansPermissions_TenantPlan_TenantPlanId",
                table: "TenantsPlansPermissions",
                column: "TenantPlanId",
                principalTable: "TenantPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantsPricingOptions_TenantPlan_TenantPlanId",
                table: "TenantsPricingOptions",
                column: "TenantPlanId",
                principalTable: "TenantPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
