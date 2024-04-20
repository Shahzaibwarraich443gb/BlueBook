    using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingBlueBook.Migrations
{
    /// <inheritdoc />
    public partial class CorporateTaxAddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CostOfSale",
                table: "CorporateTax",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherExpense",
                table: "CorporateTax",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherIncome",
                table: "CorporateTax",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostOfSale",
                table: "CorporateTax");

            migrationBuilder.DropColumn(
                name: "OtherExpense",
                table: "CorporateTax");

            migrationBuilder.DropColumn(
                name: "OtherIncome",
                table: "CorporateTax");
        }
    }
}
