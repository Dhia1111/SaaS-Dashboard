using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Connection.Migrations
{
    /// <inheritdoc />
    public partial class updateDb12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantPlans_Tenants_TenantId",
                table: "TenantPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_TenantPlans_TenantPlanId",
                table: "UserSubscriptions");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantPlan",
                table: "TenantPlan",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TenantsPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PermissionKey = table.Column<string>(type: "text", nullable: false),
                    BitValue = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantsPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantsPlansBenifests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    PlanId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantsPlansBenifests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantsPricingOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    BillingCycle = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    DurationInMonths = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantPlanId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantsPricingOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantsPricingOptions_TenantPlan_TenantPlanId",
                        column: x => x.TenantPlanId,
                        principalTable: "TenantPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantsPricingOptions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantsPlansPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    TenantPlanId = table.Column<int>(type: "integer", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantsPlansPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantsPlansPermissions_TenantPlan_TenantPlanId",
                        column: x => x.TenantPlanId,
                        principalTable: "TenantPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantsPlansPermissions_TenantsPermissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "TenantsPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantsPlansPermissions_PermissionId",
                table: "TenantsPlansPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantsPlansPermissions_TenantPlanId",
                table: "TenantsPlansPermissions",
                column: "TenantPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantsPricingOptions_TenantId",
                table: "TenantsPricingOptions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantsPricingOptions_TenantPlanId",
                table: "TenantsPricingOptions",
                column: "TenantPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantPlan_Tenants_TenantId",
                table: "TenantPlan",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_TenantPlan_TenantPlanId",
                table: "UserSubscriptions",
                column: "TenantPlanId",
                principalTable: "TenantPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantPlan_Tenants_TenantId",
                table: "TenantPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_TenantPlan_TenantPlanId",
                table: "UserSubscriptions");

            migrationBuilder.DropTable(
                name: "TenantsPlansBenifests");

            migrationBuilder.DropTable(
                name: "TenantsPlansPermissions");

            migrationBuilder.DropTable(
                name: "TenantsPricingOptions");

            migrationBuilder.DropTable(
                name: "TenantsPermissions");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantPlans",
                table: "TenantPlans",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantPlans_Tenants_TenantId",
                table: "TenantPlans",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_TenantPlans_TenantPlanId",
                table: "UserSubscriptions",
                column: "TenantPlanId",
                principalTable: "TenantPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
