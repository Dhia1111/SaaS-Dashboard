using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Connection.Migrations
{
    /// <inheritdoc />
    public partial class Update3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantsMarketting");

            migrationBuilder.AddColumn<int>(
                name: "GradeLevel",
                table: "TenantPlan",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DiscoveriesPlatforms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    TenantClientIdentifier = table.Column<string>(type: "text", nullable: false),
                    MarkettingPlatform = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscoveriesPlatforms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscoveriesPlatforms_TenantId_TenantClientIdentifier",
                table: "DiscoveriesPlatforms",
                columns: new[] { "TenantId", "TenantClientIdentifier" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscoveriesPlatforms");

            migrationBuilder.DropColumn(
                name: "GradeLevel",
                table: "TenantPlan");

            migrationBuilder.CreateTable(
                name: "TenantsMarketting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MarkettingPlatform = table.Column<int>(type: "integer", nullable: false),
                    TenantClientIdentifier = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantsMarketting", x => x.Id);
                });
        }
    }
}
