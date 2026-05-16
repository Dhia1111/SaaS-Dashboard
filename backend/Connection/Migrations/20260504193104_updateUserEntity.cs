using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connection.Migrations
{
    /// <inheritdoc />
    public partial class updateUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Persons_PersonID",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PersonID",
                table: "Users",
                newName: "PersonId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_PersonID",
                table: "Users",
                newName: "IX_Users_PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Persons_PersonId",
                table: "Users",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Persons_PersonId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PersonId",
                table: "Users",
                newName: "PersonID");

            migrationBuilder.RenameIndex(
                name: "IX_Users_PersonId",
                table: "Users",
                newName: "IX_Users_PersonID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Persons_PersonID",
                table: "Users",
                column: "PersonID",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
