using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class _TbLCUstomerUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Companies",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Companies",
                newName: "EmailAddress");

            migrationBuilder.AddColumn<int>(
                name: "EmailId",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhoneId",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_EmailId",
                table: "Companies",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_PhoneId",
                table: "Companies",
                column: "PhoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Emails_EmailId",
                table: "Companies",
                column: "EmailId",
                principalTable: "Emails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_PhoneNumbers_PhoneId",
                table: "Companies",
                column: "PhoneId",
                principalTable: "PhoneNumbers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Emails_EmailId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_PhoneNumbers_PhoneId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_EmailId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_PhoneId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "EmailId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PhoneId",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Companies",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "EmailAddress",
                table: "Companies",
                newName: "Email");
        }
    }
}
