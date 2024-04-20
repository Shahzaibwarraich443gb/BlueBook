using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class personaltaxschamespousedepremoval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dependents_PersonalTax_PersonalTaxId",
                table: "Dependents");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalTax_Spouses_SpouseId",
                table: "PersonalTax");

            migrationBuilder.DropIndex(
                name: "IX_PersonalTax_SpouseId",
                table: "PersonalTax");

            migrationBuilder.DropIndex(
                name: "IX_Dependents_PersonalTaxId",
                table: "Dependents");

            migrationBuilder.DropColumn(
                name: "DependentIds",
                table: "PersonalTax");

            migrationBuilder.DropColumn(
                name: "SpouseId",
                table: "PersonalTax");

            migrationBuilder.DropColumn(
                name: "PersonalTaxId",
                table: "Dependents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DependentIds",
                table: "PersonalTax",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SpouseId",
                table: "PersonalTax",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PersonalTaxId",
                table: "Dependents",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonalTax_SpouseId",
                table: "PersonalTax",
                column: "SpouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Dependents_PersonalTaxId",
                table: "Dependents",
                column: "PersonalTaxId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dependents_PersonalTax_PersonalTaxId",
                table: "Dependents",
                column: "PersonalTaxId",
                principalTable: "PersonalTax",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalTax_Spouses_SpouseId",
                table: "PersonalTax",
                column: "SpouseId",
                principalTable: "Spouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
