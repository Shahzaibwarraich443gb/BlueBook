using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class productServicesAccountsAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdvanceSaleTaxAccountId",
                table: "ProductServices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpenseSaleTax",
                table: "ProductServices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LiabilityAccountId",
                table: "ProductServices",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailAddress",
                table: "AbpUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvanceSaleTaxAccountId",
                table: "ProductServices");

            migrationBuilder.DropColumn(
                name: "ExpenseSaleTax",
                table: "ProductServices");

            migrationBuilder.DropColumn(
                name: "LiabilityAccountId",
                table: "ProductServices");

            migrationBuilder.AlterColumn<string>(
                name: "EmailAddress",
                table: "AbpUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);
        }
    }
}
